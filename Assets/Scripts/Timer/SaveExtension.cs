using System;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveExtension
{
    public static void Save<T>(T value, string id)
    {
        var @string = JsonConvert.SerializeObject(value);
        PlayerPrefs.SetString(id, @string);
    }

    public static T Override<T>(string id, T value)
    {
        if(PlayerPrefs.HasKey(id))
        {
            var @string = PlayerPrefs.GetString(id);
            JsonConvert.PopulateObject(@string, value);
            return value;
        }
        else
        {
            return value;
        }
    }
}