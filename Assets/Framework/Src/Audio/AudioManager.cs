using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioManagerSource
{
    public AudioSource audioSource;
    public AudioType audioType;
    public float startedAt;
    public float duration;
    public float volumeScale = 1f;
    public bool isLooping = false;
    public bool isPositional = false;
}

public class AudioManager : Singleton<AudioManager>
{
    private Dictionary<AudioType, float> audioVolume = new Dictionary<AudioType, float>();
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    private List<AudioManagerSource> runningSources = new List<AudioManagerSource>();

    public void Update()
    {
        for (int i = this.runningSources.Count - 1; i >= 0; i--) {
            if (this.runningSources[i].audioSource == null) {
                this.runningSources.RemoveAt(i);
                continue;
            }

            if (this.runningSources[i].isPositional) {
                float baseVolume = this.GetVolumeForAudioType(this.runningSources[i].audioType);
                float distanceMultiplier = this.GetVolumeMultiplierForDistance((Camera.main.transform.position - this.runningSources[i].audioSource.transform.position).magnitude);

                this.runningSources[i].audioSource.volume = baseVolume * this.runningSources[i].volumeScale * distanceMultiplier;
            }

            if (!this.runningSources[i].isLooping && this.runningSources[i].startedAt + this.runningSources[i].duration <= Time.time) {
                Destroy(this.runningSources[i].audioSource.gameObject);

                this.runningSources.RemoveAt(i);
                continue;
            }
        }
    }

    public AudioSource Play(string name)
    {
        return this.Play(name, AudioType.Music, 1f, false);
    }

    public AudioSource Play(string name, AudioType audioType)
    {
        return this.Play(name, audioType, 1f, false);
    }

    public AudioSource Play(string name, AudioType audioType, float volumeScale)
    {
        return this.Play(name, audioType, volumeScale, false);
    }

    public AudioSource Play(string name, AudioType audioType, float volumeScale, bool isLooping)
    {
        if (!this.audioClips.ContainsKey(name)) {
            Logger.MessageFormat("Playing non-preloaded sound, should be pre-loaded in the future: {0}", name);

            this.Preload(name);
        }

        GameObject tempAudioObject = new GameObject("Audio<" + name + ">");
        AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();

        tempAudioObject.transform.SetParent(this.transform, false);
        tempAudioObject.transform.position = new Vector3(0, Camera.main.transform.position.y, 0);

        tempAudioSource.clip = this.audioClips[name];
        tempAudioSource.volume = this.GetVolumeForAudioType(audioType) * volumeScale;
        tempAudioSource.loop = isLooping;
        tempAudioSource.Play();

        this.runningSources.Add(new AudioManagerSource {
            audioSource = tempAudioSource,
            audioType = audioType,
            startedAt = Time.time,
            duration = this.audioClips[name].length,
            volumeScale = volumeScale,
            isLooping = isLooping
        });

        return tempAudioSource;
    }

    public AudioSource PlayAtPosition(string name, Transform transform)
    {
        return this.PlayAtPosition(name, transform.position, AudioType.Music, 1f, false);
    }

    public AudioSource PlayAtPosition(string name, Vector3 position)
    {
        return this.PlayAtPosition(name, position, AudioType.Music, 1f, false);
    }

    public AudioSource PlayAtPosition(string name, Transform transform, AudioType audioType)
    {
        return this.PlayAtPosition(name, transform.position, audioType, 1f, false);
    }

    public AudioSource PlayAtPosition(string name, Vector3 position, AudioType audioType)
    {
        return this.PlayAtPosition(name, position, audioType, 1f, false);
    }

    public AudioSource PlayAtPosition(string name, Transform transform, AudioType audioType, float volumeScale)
    {
        return this.PlayAtPosition(name, transform.position, audioType, volumeScale, false);
    }

    public AudioSource PlayAtPosition(string name, Vector3 position, AudioType audioType, float volumeScale)
    {
        return this.PlayAtPosition(name, position, audioType, volumeScale, false);
    }

    public AudioSource PlayAtPosition(string name, Transform transform, AudioType audioType, float volumeScale, bool isLooping)
    {
        return this.PlayAtPosition(name, transform.position, audioType, volumeScale, isLooping);
    }

    public AudioSource PlayAtPosition(string name, Vector3 position, AudioType audioType, float volumeScale, bool isLooping)
    {
        if (!this.audioClips.ContainsKey(name)) {
            Logger.MessageFormat("Playing non-preloaded sound, should be pre-loaded in the future: " + name);

            this.Preload(name);
        }

        GameObject tempAudioObject = new GameObject("AudioInWorld<" + name + ">");
        AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();

        tempAudioObject.transform.SetParent(this.transform, false);
        tempAudioObject.transform.position = new Vector3(position.x, Camera.main.transform.position.y, position.z);

        float baseVolume = this.GetVolumeForAudioType(audioType);
        float distanceMultiplier = this.GetVolumeMultiplierForDistance((Camera.main.transform.position - position).magnitude);

        tempAudioSource.clip = this.audioClips[name];
        tempAudioSource.volume = baseVolume * volumeScale * distanceMultiplier;
        tempAudioSource.loop = isLooping;
        tempAudioSource.Play();

        this.runningSources.Add(new AudioManagerSource {
            audioSource = tempAudioSource,
            audioType = audioType,
            startedAt = Time.time,
            duration = this.audioClips[name].length,
            volumeScale = volumeScale,
            isPositional = true,
            isLooping = isLooping
        });

        return tempAudioSource;
    }

    public AudioSource PlayAndParentTo(string name, Transform transform)
    {
        return this.PlayAndParentTo(name, transform, AudioType.Music, 1f, false);
    }

    public AudioSource PlayAndParentTo(string name, Transform transform, AudioType audioType)
    {
        return this.PlayAndParentTo(name, transform, audioType, 1f, false);
    }

    public AudioSource PlayAndParentTo(string name, Transform transform, AudioType audioType, float volumeScale)
    {
        return this.PlayAndParentTo(name, transform, audioType, 1f, false);
    }

    public AudioSource PlayAndParentTo(string name, Transform transform, AudioType audioType, float volumeScale, bool isLooping)
    {
        if (!this.audioClips.ContainsKey(name)) {
            Logger.MessageFormat("Playing non-preloaded sound, should be pre-loaded in the future: " + name);

            this.Preload(name);
        }

        GameObject tempAudioObject = new GameObject("AudioInParent<" + name + ">");
        AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();
        Vector3 position = transform.position;

        tempAudioObject.transform.SetParent(transform, false);
        tempAudioObject.transform.position = new Vector3(position.x, Camera.main.transform.position.y, position.z);
        tempAudioObject.transform.rotation = Quaternion.identity;

        float baseVolume = this.GetVolumeForAudioType(audioType);
        float distanceMultiplier = this.GetVolumeMultiplierForDistance((Camera.main.transform.position - position).magnitude);

        tempAudioSource.clip = this.audioClips[name];
        tempAudioSource.volume = baseVolume * volumeScale * distanceMultiplier;
        tempAudioSource.loop = isLooping;
        tempAudioSource.Play();

        this.runningSources.Add(new AudioManagerSource {
            audioSource = tempAudioSource,
            audioType = audioType,
            startedAt = Time.time,
            duration = this.audioClips[name].length,
            volumeScale = volumeScale,
            isPositional = true,
            isLooping = isLooping
        });

        return tempAudioSource;
    }

    public AudioSource PlayLooping(string name)
    {
        return this.PlayLooping(name, AudioType.Music, 1f);
    }

    public AudioSource PlayLooping(string name, AudioType audioType)
    {
        return this.PlayLooping(name, audioType, 1f);
    }

    public AudioSource PlayLooping(string name, AudioType audioType, float volumeScale)
    {
        if (!this.audioClips.ContainsKey(name)) {
            Logger.MessageFormat("Playing non-preloaded sound, should be pre-loaded in the future: " + name);

            this.Preload(name);
        }

        GameObject tempAudioObject = new GameObject("AudioLooping<" + name + ">");
        AudioSource tempAudioSource = tempAudioObject.AddComponent<AudioSource>();

        tempAudioObject.transform.SetParent(this.transform, false);
        tempAudioObject.transform.position = new Vector3(0, Camera.main.transform.position.y, 0);

        tempAudioSource.clip = this.audioClips[name];
        tempAudioSource.volume = this.GetVolumeForAudioType(audioType) * volumeScale;
        tempAudioSource.loop = true;
        tempAudioSource.Play();

        this.runningSources.Add(new AudioManagerSource {
            audioSource = tempAudioSource,
            audioType = audioType,
            startedAt = Time.time,
            duration = this.audioClips[name].length,
            volumeScale = volumeScale,
            isLooping = true
        });

        return tempAudioSource;
    }

    public void Stop(AudioSource audioSource)
    {
        Destroy(audioSource.gameObject);
    }

    public void Preload(string name)
    {
        if (this.audioClips.ContainsKey(name)) {
            return;
        }

        AudioClip audioClip = Resources.Load<AudioClip>(name);

        this.audioClips.Add(name, audioClip);
    }

    public float GetVolumeForAudioType(AudioType audioType)
    {
        float volume = 0;

        if (!this.audioVolume.TryGetValue(audioType, out volume)) {
            volume = 1f;
        }

        return volume;
    }

    public void SetVolumeForAudioType(AudioType audioType, float volume)
    {
        this.audioVolume[audioType] = volume;
    }

    public float GetVolumeMultiplierForDistance(float distance)
    {
        return Mathf.Lerp(1f, 0, distance / 15f);
    }
}
