using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Lunatic.Timer
{
    public delegate void OnFinish();

    public interface ITickable
    {
        void Run(CancellationToken token);
        void AddTicks(TimeSpan value);
        void SubtructTicks(TimeSpan value);

        TimeSpan GetTimeLeft();

        event OnFinish OnTimerEnd;
    }
}