using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;

public class AssetManager : Singleton<AssetManager>
{
    private Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

    public bool HasBundle(string name)
    {
        return this.assetBundles.ContainsKey(name);
    }

    public AssetBundle GetBundle(string name)
    {
        if (!this.HasBundle(name)) {
            return null;
        }

        return this.assetBundles[name];
    }

    public T GetAssetFromBundle<T>(string name, string path) where T : UnityEngine.Object
    {
        if (!this.HasBundle(name)) {
            return null;
        }

        string fixedPath = path.ToLower();

        if (!this.assetBundles[name].Contains(fixedPath)) {
            Logger.MessageFormat("Couldn't find `{0}` in bundle, available assets: {1}", fixedPath, string.Join(", ", this.assetBundles[name].GetAllAssetNames()));

            return null;
        }

        return this.assetBundles[name].LoadAsset<T>(fixedPath);
    }

    public void LoadRemoteAssetBundle(string name, string path, uint version)
    {
        StartCoroutine(this.LoadRemoteAssetBundleEnumerator(name, path, version, null, null));
    }

    public void LoadRemoteAssetBundle(string name, string path, uint version, Action<bool> onLoaded, Action<string> onError)
    {
        StartCoroutine(this.LoadRemoteAssetBundleEnumerator(name, path, version, onLoaded, onError));
    }

    private IEnumerator LoadRemoteAssetBundleEnumerator(string name, string downloadPath, uint version, Action<bool> onLoaded, Action<string> onError)
    {
        float startedAt = Time.time;

        while (!Caching.ready) {
            yield return null;
        }

        using (UnityWebRequest bundleRequest = UnityWebRequest.GetAssetBundle(downloadPath, version, 0)) {
            yield return bundleRequest.Send();

            if (bundleRequest.isHttpError || bundleRequest.isNetworkError) {
                Logger.ErrorFormat("Failed to load asset bundle `{0}`: {1}", name, bundleRequest.error);

                if (onError != null) {
                    onError(bundleRequest.error);
                }

                yield break;
            }

            AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(bundleRequest);

            if (assetBundle == null) {
                Logger.ErrorFormat("Failed to load asset bundle `{0}`: bundle was null...", name);

                if (onError != null) {
                    onError(bundleRequest.error);
                }

                yield break;
            }

            this.assetBundles.Add(name, assetBundle);

            Logger.MessageFormat("Finished loading asset bundle `{0}` in {1}ms...", name, (Time.time - startedAt).ToString("n2"));

            if (onLoaded != null) {
                onLoaded(true);
            }
        }
    }
}
