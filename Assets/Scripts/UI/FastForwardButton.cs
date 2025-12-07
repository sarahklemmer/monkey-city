using UnityEngine;
using UnityEngine.UI;

public class FastForwardButton : MonoBehaviour
{
    public bool spedUp { get; private set; } = false;
    public static FastForwardButton instance;
    [SerializeField] Image icon;
    [SerializeField] Sprite singleArrow; 
    [SerializeField] Sprite doubleArrow;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate FastForwardButton on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        Debug.Log("WaveSpawner: Instance created and ready");
    }

    void Update()
    {
        icon.sprite = spedUp ? doubleArrow : singleArrow;
    }

    public void Click()
    {
        if(spedUp) ReturnToNormalSpeed();
        else FastForward();
    }

    public void FastForward()
    {
        if(spedUp) return;
        TimeController.instance.DoubleSpeed();
        BananaProductionTimer.instance.DoubleSpeed();
        AllMonkeyInfo.instance.IncreaseMonkeySpeed(5f);
        
        spedUp = true;
    }

    public void ReturnToNormalSpeed()
    {
        if(!spedUp) return;
        TimeController.instance.HalfSpeed();
        BananaProductionTimer.instance.HalfSpeed();
        AllMonkeyInfo.instance.IncreaseMonkeySpeed(-5f);

        spedUp = false;
    }
}
