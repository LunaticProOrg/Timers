using UnityEngine;

namespace Lunatic.Timer
{
    [CreateAssetMenu(menuName = "Config/Timer_Settings")]
    public class TimerSettings : ScriptableObject
    {
        [field: SerializeField] public int DefaultTimerInSeconds { get; private set; }
        [field: SerializeField] public float IncrementalDuration { get; private set; }
        [field: SerializeField] public float IncrementalSpeed { get; private set; }
        [field: SerializeField] public int TimerSecondsChangesValue { get; private set; }
    }
}
