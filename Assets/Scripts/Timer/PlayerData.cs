using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public int buttonsCount;
    public Dictionary<int, int> timers;

    public int? this[int index]
    {
        get
        {
            if(timers.ContainsKey(index)) return timers[index];

            else return null;
        }
    }
}
