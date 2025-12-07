using UnityEngine;
using UnityEngine.UIElements;

public class TimeController : MonoBehaviour
{
    private float secondsPassedSinceLastDay = 0;
    private bool ticking = true;
    private bool night = false;
    public int currentDay { get; private set; }
    [SerializeField]float secondsToDay = 30;
    private bool doubled = false; 

    public static TimeController instance;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate TimeController on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        secondsToDay = 30;
    }

    void Update()
    {
        if (ticking) secondsPassedSinceLastDay += Time.deltaTime;
        else return;

        if (secondsPassedSinceLastDay >= (secondsToDay / 2f) && !night)
        {
            night = true;
            DayNightToggle.instance.SetNight(true);
        }
        if (secondsPassedSinceLastDay >= secondsToDay)
        {
            PassDay();
        }
    }

    public void StopTicking()
    {
        ticking = false;
    }

    public void StartTicking()
    {
        ticking = true;
    }

    public void HalfSpeed()
    {
        if(!doubled) return;
        doubled = false;
        secondsToDay *= 2;
    }

    public void DoubleSpeed()
    {
        if(doubled) return;
        doubled = true;
        secondsToDay /= 2;
    }

    void PassDay()
    {
        ++currentDay;
        secondsPassedSinceLastDay %= secondsToDay;
        night = false;
        DayNightToggle.instance.SetNight(false);
    }
}
