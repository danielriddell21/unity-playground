public class CooldownTimer
{
    public float TimeRemaining { get; private set; }
    public float TotalTime { get; private set; }
    public bool IsRecurring { get; }
    public bool IsActive { get; private set; }

    public float TimeElapsed => TotalTime - TimeRemaining;
    public float PercentElapsed => TimeElapsed / TotalTime;
    public bool IsCompleted => TimeRemaining <= 0;

    public delegate void TimerCompleteHandler();

    public event TimerCompleteHandler TimerCompleteEvent;

    public CooldownTimer(float time, bool recurring = false)
    {
        TotalTime = time;
        IsRecurring = recurring;
        TimeRemaining = TotalTime;
    }

    public void StartTimer()
    {
        TimeRemaining = TotalTime;
        IsActive = true;
    }

    public void StartTimer(float time)
    {
        TotalTime = time;
        StartTimer();
    }

    public void Update(float deltaTime)
    {
        if (!IsActive)
        {
            return;
        }

        if (TimeRemaining > 0) 
        {
            TimeRemaining -= deltaTime;
        }

        if (TimeRemaining <= 0)
        {
            if (IsRecurring)
            {
                TimeRemaining = TotalTime;
            }
            else
            {
                IsActive = false;
            }

            TimerCompleteEvent?.Invoke();
        }
    }

    public void Pause()
    {
        IsActive = false;
    }

    public void AddTime(float time)
    {
        TimeRemaining += time;
        TotalTime += time;
    }
}