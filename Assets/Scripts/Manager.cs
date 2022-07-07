using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private const string saveKey = "saveKey";

    private PlayerData _data;

    public PlayerData Data => _data;

    public void Load()
    {
        _data = SaveExtension.Override(saveKey, new PlayerData());
        _data.timers ??= new Dictionary<int, int>();
    }

    public void Save()
    {
        SaveExtension.Save(_data, saveKey);
    }
}
