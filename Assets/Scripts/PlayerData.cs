using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public Dictionary<int, double> timers;

    public double? this[int index]
    {
        get
        {
            if(timers.ContainsKey(index)) return timers[index];

            else return null;
        }
    }
}
