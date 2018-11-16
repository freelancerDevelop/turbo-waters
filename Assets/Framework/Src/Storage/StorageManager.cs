using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : Singleton<StorageManager>
{
    public float GetFloat(string name, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(name, defaultValue);
    }

    public void SetFloat(string name, float value)
    {
        PlayerPrefs.SetFloat(name, value);
        PlayerPrefs.Save();
    }

    public int GetInt(string name, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(name, defaultValue);
    }

    public void SetInt(string name, int value)
    {
        PlayerPrefs.SetInt(name, value);
        PlayerPrefs.Save();
    }

    public bool GetBool(string name, bool defaultValue = false)
    {
        return PlayerPrefs.GetInt(name, defaultValue ? 1 : 0) == 1;
    }

    public void SetBool(string name, bool value)
    {
        PlayerPrefs.SetInt(name, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public string GetString(string name, string defaultValue = null)
    {
        return PlayerPrefs.GetString(name, defaultValue);
    }

    public void SetString(string name, string value)
    {
        PlayerPrefs.SetString(name, value);
        PlayerPrefs.Save();
    }
}
