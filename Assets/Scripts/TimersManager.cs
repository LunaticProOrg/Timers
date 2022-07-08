using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Lunatic.Timer
{
    public class TimersManager : MonoBehaviour
    {
        [SerializeField] private TimerSettings _defaultSettings;
        [SerializeField] private TimerMenuManager _timerMenuManager;
        [SerializeField] private TimerBanner _banner;

        private SaveLoadManager _dataManager;
        private Dictionary<int, ITickable> _tickables;

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _dataManager = new SaveLoadManager();
            _dataManager.Load();
            _tickables ??= new Dictionary<int, ITickable>();

            _timerMenuManager.Initialize(_dataManager.Data, _defaultSettings, (int index) => SetTickable(index));
            _banner.Initialize(_defaultSettings);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();

            foreach(var kvp in _tickables)
            {
                _dataManager.Data.timers[kvp.Key] = _tickables.ContainsKey(kvp.Key)
                    ? _tickables[kvp.Key].GetTimeLeft().TotalMilliseconds
                    : _defaultSettings.DefaultTimerInSeconds;
            }

            _dataManager.Save();
        }

        public void SetTickable(int index)
        {
            InitializeTickables(index);

            _banner.SetTickable(_tickables[index], _cancellationTokenSource.Token, _timerMenuManager.ShowButtons);
        }

        private void InitializeTickables(int index)
        {
            if(!_tickables.ContainsKey(index))
            {
                var timer = new Timer(TimeSpan.FromSeconds(_defaultSettings.DefaultTimerInSeconds));
                _tickables.Add(index, timer);
            }
        }
    }
}