using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lunatic.Timer
{
    public class LinearSmoothValueIncremental
    {
        private readonly float _baseDuration;
        private readonly float _speed;

        private bool _stayingInIncremental;

        private float _elapsed;
        private float _currentDuration;

        public LinearSmoothValueIncremental(float baseDuration, float speed)
        {
            _stayingInIncremental = false;
            _baseDuration = baseDuration;
            _speed = speed;
        }

        public void OnIncrementalEnter()
        {
            _stayingInIncremental = true;
            _currentDuration = _baseDuration;
            _elapsed = 0f;
        }

        public void OnIncrementalExit()
        {
            _stayingInIncremental = false;
            _currentDuration = _baseDuration;
            _elapsed = 0f;
        }

        public void OnIncrementalStay(Action OnTickCallback)
        {
            if(!_stayingInIncremental) return;

            if(_elapsed < 1f)
            {
                _elapsed += Time.deltaTime / _currentDuration;
            }
            else
            {
                _elapsed = 0f;
                _currentDuration = Mathf.Clamp(_currentDuration / _speed, 0f, _baseDuration);

                OnTickCallback?.Invoke();
            }

        }
    }
}
