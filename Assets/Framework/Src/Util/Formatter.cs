using UnityEngine;
using System;
using Newtonsoft.Json;

public class Formatter : Singleton<Formatter>
{
    public T FromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public string ToJson<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public string ToJson<T>(T obj, Formatting formatting)
    {
        return JsonConvert.SerializeObject(obj, formatting);
    }

    public string DurationToClock(float duration)
    {
        return this.DurationToClock((uint) duration);
    }

    public string DurationToClock(int duration)
    {
        return this.DurationToClock((uint) duration);
    }

    public string DurationToClock(uint duration)
    {
        uint hours = ~~(duration / 3600);
        uint minutes = ~~((duration % 3600) / 60);
        uint seconds = duration % 60;
        string result = "";

        if (hours > 0) {
            result += hours + ":" + (minutes < 10 ? "0" : "");
        }

        result += minutes + ":" + (seconds < 10 ? "0" : "");
        result += seconds;

        return result;
    }
}
