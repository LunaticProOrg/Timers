using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PressedButtonType
{
    add = 0,
    subtract = 1
}


public class TimerBanner : MonoBehaviour
{
    [SerializeField] private Manager _manager;
    [SerializeField] private RectTransform banner;
    [SerializeField] private TextMeshProUGUI _text;

    [SerializeField] private Button AddTimeButton;
    [SerializeField] private Button SubrtuctTimeButton;
    [SerializeField] private Button StartTickButton;
    [SerializeField] private Button CloseTimerButton;

    [SerializeField] private Color OnTimerEndButtonHighLightColor;

    [SerializeField] private float OnTimerEndDuration;
    [SerializeField] private float BackColorDuration;
    [SerializeField] private float BannerAnimationDelay;
    [SerializeField] private float TimeIncrementDuration;
    [SerializeField] private float valueForDecrementTime;


    [SerializeField] private int _defaultTimerSeconds;
    [SerializeField] private int secondsToAdd, secondsToSubrtruct;

    private ITickable tickable;
    private Dictionary<int, ITickable> tickables;
    private CancellationTokenSource _cancellationTokenSource;

    private float elapsedAdd;
    private float elapsedSubtract;
    private float defaultTimeIncrement;

    private bool heldAdd;
    private bool heldSubtruct;

    private void Awake()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        _manager.Load();
        defaultTimeIncrement = TimeIncrementDuration;

        tickables = new Dictionary<int, ITickable>();

        banner.DOScale(Vector3.zero, 0f);
    }

    private void CreateTimer(int index)
    {
        var viewable = new TextViewer(_text);
        int time = _manager.Data[index] == null ? _defaultTimerSeconds : _manager.Data[index].Value;

        if(!tickables.ContainsKey(index))
        {
            var timer = new Timer(viewable, time, _cancellationTokenSource.Token);
            tickables.Add(index, timer);
        }

        tickable = tickables[index];
        tickable.SetViewable(viewable);


        if(_manager.Data[index] == null)
        {
            _manager.Data.timers.Add(index, tickable.GetCurrentTimeLeft);
        }

        SetupButtons();

        tickable.EndTicking += Tickable_EndTicking;
    }

    private void Tickable_EndTicking()
    {
        Color defaultColor = StartTickButton.targetGraphic.color;

        StartTickButton.targetGraphic.rectTransform.DOShakeScale(OnTimerEndDuration);
        StartTickButton.targetGraphic.DOBlendableColor(OnTimerEndButtonHighLightColor, OnTimerEndDuration).OnComplete(() =>
        {
            StartTickButton.targetGraphic.DOColor(defaultColor, BackColorDuration);
        });
    }

    private void LateUpdate()
    {
        if(tickable != null && tickable.IsTimerTick())
            tickable?.Tick();
    }

    private void Update()
    {
        IncrementAdd();
        IncrementSubtract();
    }

    private void IncrementAdd()
    {
        if(heldAdd)
        {
            if(elapsedAdd < 1f)
            {
                elapsedAdd += Time.deltaTime / TimeIncrementDuration;
            }
            else
            {
                TimeIncrementDuration /= valueForDecrementTime;
                TimeIncrementDuration = Mathf.Clamp(TimeIncrementDuration, 0f, defaultTimeIncrement);
                elapsedAdd = 0f;

                tickable.AddTicks(secondsToAdd);
            }
        }
    }

    private void IncrementSubtract()
    {
        if(heldSubtruct)
        {
            if(elapsedSubtract < 1f)
            {
                elapsedSubtract += Time.deltaTime / TimeIncrementDuration;
            }
            else
            {
                TimeIncrementDuration /= valueForDecrementTime;
                TimeIncrementDuration = Mathf.Clamp(TimeIncrementDuration, 0f, defaultTimeIncrement);
                elapsedSubtract = 0f;


                tickable.SubtructTicks(secondsToSubrtruct);
            }
        }
    }

    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();


        foreach(var kvp in tickables)
        {
            var i = kvp.Key;

            _manager.Data.timers[i] = tickables.ContainsKey(i) ? tickables[i].GetCurrentTimeLeft : _defaultTimerSeconds;
        }

        _manager.Save();
    }

    private void SetupButtons()
    {
        StartTickButton.onClick.RemoveAllListeners();
        AddTimeButton.onClick.RemoveAllListeners();
        SubrtuctTimeButton.onClick.RemoveAllListeners();
        CloseTimerButton.onClick.RemoveAllListeners();

        StartTickButton.onClick.AddListener(() => tickable.StartTimer());
        AddTimeButton.onClick.AddListener(() => tickable.AddTicks(secondsToAdd));
        SubrtuctTimeButton.onClick.AddListener(() => tickable.SubtructTicks(secondsToSubrtruct));
        CloseTimerButton.onClick.AddListener(() => CloseBanner());
    }

    public void ShowBanner(int index)
    {
        CreateTimer(index);

        banner.DOScale(Vector3.one, BannerAnimationDelay).OnComplete(() =>
       {
           banner.DOPunchScale(Vector3.one / 3f, BannerAnimationDelay);
       });
    }

    private void CloseBanner()
    {
        tickable.EndTicking -= Tickable_EndTicking;
        banner.DOScale(Vector3.zero, BannerAnimationDelay);
        tickable = null;

        GetComponent<MenuManager>().Show();
    }

    public void OnDown(int type)
    {
        heldAdd = (PressedButtonType)type == PressedButtonType.add ? true : false;
        heldSubtruct = (PressedButtonType)type == PressedButtonType.subtract ? true : false;
    }

    public void OnUp(int type)
    {
        switch((PressedButtonType)type)
        {
            case PressedButtonType.add:
                heldAdd = false;
                elapsedAdd = 0f;
                break;
            case PressedButtonType.subtract:
                heldSubtruct = false;
                elapsedSubtract = 0f;
                break;
        }

        TimeIncrementDuration = defaultTimeIncrement;
    }
}

