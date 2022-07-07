using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;

public delegate void OnEndTicking();

public class Timer : ITickable
{
    protected int seconds { get; private set; }
    protected IViewable viewable { get; private set; }

    protected int currentSeconds;

    protected string currentTime;

    public event OnEndTicking EndTicking;

    public int GetCurrentTimeLeft => seconds - currentSeconds;

    private bool _isRunning;


    CancellationToken token;

    public Timer(IViewable viewable, int seconds, CancellationToken token)
    {
        currentTime = string.Empty;
        currentSeconds = 0;
        _isRunning = false;
        this.viewable = viewable;
        this.seconds = seconds;
        this.token = token;

        AddTimeToQueue(seconds);
        Tick();
    }

    public void SetViewable(IViewable viewable)
    {
        this.viewable = viewable;

        currentTime = string.Empty;

        AddTimeToQueue(seconds - currentSeconds);
        Tick();
    }

    public void StartTimer()
    {
        if(!token.IsCancellationRequested && !IsTimerTick())
            CreateTimer(token);
    }

    private async void CreateTimer(CancellationToken token)
    {
        _isRunning = true;

        try
        {
            while(currentSeconds < seconds && !token.IsCancellationRequested)
            {
                var overrideCurrentTimeScale = 1f / Time.timeScale;

                await Task.Delay(Mathf.RoundToInt(1000f * overrideCurrentTimeScale), token);

                currentSeconds++;

                var time = seconds - currentSeconds;

                AddTimeToQueue(time);
            }

            EndTicking?.Invoke();


            currentTime = string.Empty;

            AddTimeToQueue(0);
            Tick();

            _isRunning = false;

        }
        catch(Exception ex)
        {
            Debug.Log(ex);
        }
    }

    private void AddTimeToQueue(int time)
    {
        if(time < 0) time = 0;

        var t = TimeSpan.FromSeconds(time);
        currentTime = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
    }

    public void Tick()
    {
        if(currentTime != string.Empty)
            viewable?.Viewing(currentTime);
    }

    public void AddTicks(int value)
    {
        currentTime = string.Empty;

        seconds += value;

        AddTimeToQueue(seconds - currentSeconds);
        Tick();
    }

    public void SubtructTicks(int value)
    {
        currentTime = string.Empty;

        seconds -= value;
        seconds = Mathf.Clamp(seconds, 0, int.MaxValue);

        AddTimeToQueue(seconds - currentSeconds);
        Tick();
    }

    public bool IsTimerTick()
    {
        return _isRunning;
    }
}
