using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Lunatic.Timer
{
    public class TimerBanner : MonoBehaviour
    {
        [SerializeField] private RectTransform _banner;

        [SerializeField] private TextMeshProUGUI _timerText;

        [SerializeField] private Button _addTime_Btn;
        [SerializeField] private Button _subtractTime_Btn;
        [SerializeField] private Button _startTimer_Btn;

        [SerializeField] private Color OnTimerEndButtonHighLightColor;

        [SerializeField] private float OnTimerEndDuration;
        [SerializeField] private float BackColorDuration;
        [SerializeField] private float BannerAnimationDelay;

        private LinearSmoothValueIncremental _incremental_Add;
        private LinearSmoothValueIncremental _incremental_Subtract;

        private TimerSettings _settings;
        private ITickable _tickable;
        private IViewable _viewable;

        private string _timeFormat;

        public void Initialize(TimerSettings settings)
        {
            _settings = settings;
            _viewable = new TextViewer(_timerText);
            _timeFormat = @"dd\:hh\:mm\:ss\.ff";

            _incremental_Add = new LinearSmoothValueIncremental(_settings.IncrementalDuration, _settings.IncrementalSpeed);
            _incremental_Subtract = new LinearSmoothValueIncremental(_settings.IncrementalDuration, _settings.IncrementalSpeed);

            SetupTrigger(_addTime_Btn, EventTriggerType.PointerDown, _incremental_Add.OnIncrementalEnter);
            SetupTrigger(_addTime_Btn, EventTriggerType.PointerUp, _incremental_Add.OnIncrementalExit);

            SetupTrigger(_subtractTime_Btn, EventTriggerType.PointerDown, _incremental_Subtract.OnIncrementalEnter);
            SetupTrigger(_subtractTime_Btn, EventTriggerType.PointerUp, _incremental_Subtract.OnIncrementalExit);
        }

        private void Update()
        {
            if(_tickable == null || _viewable == null) return;

            _viewable?.Viewing(_tickable.GetTimeLeft().ToString(_timeFormat));

            _incremental_Add.OnIncrementalStay(() => _tickable?.AddTicks(TimeSpan.FromSeconds(_settings.TimerSecondsChangesValue)));
            _incremental_Subtract.OnIncrementalStay(() => _tickable?.SubtructTicks(TimeSpan.FromSeconds(_settings.TimerSecondsChangesValue)));
        }

        private void SetupTrigger(Button btn, EventTriggerType type, Action triggerCallback)
        {
            EventTrigger trigger = btn.GetComponent<EventTrigger>() ?? btn.gameObject.AddComponent<EventTrigger>();
            var onDownBtn = new EventTrigger.Entry();
            onDownBtn.eventID = type;
            onDownBtn.callback?.AddListener((e) => triggerCallback?.Invoke());
            trigger.triggers.Add(onDownBtn);
        }

        public void SetTickable(ITickable tickable, CancellationToken token, Action OnCloseBanner)
        {
            Unsabscribe();

            _tickable = tickable;
            Sabscribe();

            _addTime_Btn.onClick.RemoveAllListeners();
            _addTime_Btn.onClick.AddListener(() => tickable.AddTicks(TimeSpan.FromSeconds(_settings.TimerSecondsChangesValue)));

            _subtractTime_Btn.onClick.RemoveAllListeners();
            _subtractTime_Btn.onClick.AddListener(() => tickable.SubtructTicks(TimeSpan.FromSeconds(_settings.TimerSecondsChangesValue)));

            _startTimer_Btn.onClick.RemoveAllListeners();
            _startTimer_Btn.onClick.AddListener(() =>
            {
                tickable.Run(token);
                CloseBanner();
                OnCloseBanner?.Invoke();
            });

            ShowBanner();
        }

        private void OnTimerEnd()
        {
            Color defaultColor = _startTimer_Btn.targetGraphic.color;

            _startTimer_Btn.targetGraphic.rectTransform.DOShakeScale(OnTimerEndDuration, fadeOut: false);
            _startTimer_Btn.targetGraphic.DOBlendableColor(OnTimerEndButtonHighLightColor, OnTimerEndDuration).OnComplete(() =>
            {
                _startTimer_Btn.targetGraphic.DOColor(defaultColor, BackColorDuration);
            });
        }

        private void ShowBanner()
        {
            _banner.DOScale(Vector3.one, BannerAnimationDelay);
        }

        private void CloseBanner()
        {
            Unsabscribe();
            _banner.DOScale(Vector3.zero, BannerAnimationDelay);
            _tickable = null;
        }

        private void Sabscribe()
        {
            _tickable.OnTimerEnd += OnTimerEnd;
        }

        private void Unsabscribe()
        {
            if(_tickable == null) return;

            _tickable.OnTimerEnd -= OnTimerEnd;
        }

        private void OnDestroy()
        {
            Unsabscribe();
        }
    }
}