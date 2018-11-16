using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class TransitionManager : Singleton<TransitionManager>
{
    private Scene currentScene;
    private AsyncOperation loadingOperation;
    private float loadingProgress = 0;

    public void Start()
    {
        this.currentScene = SceneManager.GetActiveScene();
    }

    public float GetLoadingProgress()
    {
        return this.loadingProgress;
    }

    public void LoadScene(string name, bool showLoadingScreen = true, LoadSceneMode loadMode = LoadSceneMode.Single)
    {
        StartCoroutine(this.LoadSceneRoutine(name, showLoadingScreen, loadMode));
    }

    private IEnumerator LoadSceneRoutine(string name, bool showLoadingScreen, LoadSceneMode loadMode)
    {
        this.loadingProgress = 0;

        if (loadMode == LoadSceneMode.Single && this.currentScene.name != "LoadingScene" && showLoadingScreen) {
            this.loadingOperation = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);

            while (!this.loadingOperation.isDone) {
                yield return null;
            }
        }

        this.loadingOperation = SceneManager.LoadSceneAsync(name, loadMode);
        this.loadingOperation.allowSceneActivation = false;

        while (this.loadingOperation.progress < 0.9f) {
            this.loadingProgress = loadingOperation.progress;

            yield return null;
        }

        this.loadingProgress = loadingOperation.progress;

        yield return new WaitForSecondsRealtime(0.1f);

        this.loadingOperation.allowSceneActivation = true;

        while (!this.loadingOperation.isDone) {
            this.loadingProgress = 1;

            yield return null;
        }

        this.currentScene = SceneManager.GetSceneByName(name);

        EventManager.Instance.Trigger(new SceneLoadedEvent {
            name = name
        });
    }
}
