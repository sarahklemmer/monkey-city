using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float secondsPassedSinceLastDay = 0;
    private bool ticking = true;
    private bool night = false;
    public int currentDay { get; private set; }
    [SerializeField] float minutesPerDay = 8f;
    float secondsToDay;

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
        secondsToDay = 60 * minutesPerDay;
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

    void PassDay()
    {
        ++currentDay;
        secondsPassedSinceLastDay %= secondsToDay;
        night = false;
        DayNightToggle.instance.SetNight(false);
        BuildingManager.instance.PassDay();
    }
}
