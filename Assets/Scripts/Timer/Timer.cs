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

        public Timer(TimeSpan defaultTime)
        {
            _defaultTime = defaultTime;
        }

        public async void Run(CancellationToken token)
        {
            var millisecond = TimeSpan.FromMilliseconds(100);

            try
            {
                while(_currentTime.TotalMilliseconds < _defaultTime.TotalMilliseconds && !token.IsCancellationRequested)
                {
                    await Task.Delay(millisecond, token);

                    _currentTime = _currentTime.Add(millisecond);
                }

                OnTimerEnd?.Invoke();
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
        }

        public void AddTicks(TimeSpan value)
        {
            _defaultTime = _defaultTime.Add(value);
        }

        public void SubtructTicks(TimeSpan value)
        {
            _defaultTime = _defaultTime.Subtract(value);

            if(_defaultTime.TotalMilliseconds < TimeSpan.Zero.TotalMilliseconds)
                _defaultTime = TimeSpan.Zero;
        }

        public TimeSpan GetTimeLeft()
        {
            return _defaultTime - _currentTime;
        }
    }
}
