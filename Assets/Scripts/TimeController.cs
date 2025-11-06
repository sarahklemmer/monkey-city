using UnityEngine;

public class TimeController : MonoBehaviour
{
    private float secondsPassedSinceLastDay = 0;
    private bool ticking = true;
    public int currentDay { get; private set; }
    [SerializeField] float minutesPerDay = 8f;
    float secondsToDay;

    void Start()
    {
        secondsToDay = 60 * minutesPerDay;
    }

    void Update()
    {
        if(ticking) secondsPassedSinceLastDay += Time.deltaTime;
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
        BuildingManager.instance.PassDay();
    }
}
