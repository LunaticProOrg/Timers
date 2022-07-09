using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Lunatic.Timer
{
    public class Timer : ITickable
    {
        private TimeSpan _defaultTime;
        protected TimeSpan _currentTime;

        public event OnFinish OnTimerEnd;

        private bool _running;

        public Timer(TimeSpan defaultTime)
        {
            _defaultTime = defaultTime;
            _running = false;
        }

        public async void Run(CancellationToken token)
        {
            if(_running) return;

            _running = true;
            var millisecond = TimeSpan.FromMilliseconds(100);

            try
            {
                while(_currentTime.TotalMilliseconds < _defaultTime.TotalMilliseconds && !token.IsCancellationRequested)
                {
                    await Task.Delay(millisecond, token);

                    _currentTime = _currentTime.Add(millisecond);
                }

                OnTimerEnd?.Invoke();
                _running = false;
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
        }

        public void AddTicks(TimeSpan value)
        {
            _defaultTime = TimeSpan.FromMilliseconds(_defaultTime.TotalMilliseconds + value.TotalMilliseconds);
        }

        public void SubtructTicks(TimeSpan value)
        {
            _defaultTime = TimeSpan.FromMilliseconds(_defaultTime.TotalMilliseconds - value.TotalMilliseconds);

            if(_defaultTime.TotalMilliseconds < TimeSpan.Zero.TotalMilliseconds)
                _defaultTime = TimeSpan.FromMilliseconds(0);

            Debug.LogError("subtracting ticks");
        }

        public TimeSpan GetTimeLeft()
        {
            return _defaultTime - _currentTime;
        }
    }
}
