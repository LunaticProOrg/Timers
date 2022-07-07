using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ITickable
{
    void SetViewable(IViewable viewable);

    bool IsTimerTick();

    void StartTimer();
    void Tick();

    void AddTicks(int seconds);

    void SubtructTicks(int seconds);

    int GetCurrentTimeLeft { get; }

    event OnEndTicking EndTicking;
}