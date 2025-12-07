using UnityEngine;
using UnityEngine.UI;

public class FastForwardButton : MonoBehaviour
{
    public bool spedUp { get; private set; } = false;
    [SerializeField] Image icon;
    [SerializeField] Sprite singleArrow; 
    [SerializeField] Sprite doubleArrow;

    void Update()
    {
        icon.sprite = spedUp ? doubleArrow : singleArrow;
    }

    public void Click()
    {
        if(spedUp) ReturnToNormal();
        else FastForward();
    }

    void FastForward()
    {
        TimeController.instance.DoubleSpeed();
        BananaProductionTimer.instance.DoubleSpeed();
        
        spedUp = true;
    }

    void ReturnToNormal()
    {
        TimeController.instance.HalfSpeed();
        BananaProductionTimer.instance.HalfSpeed();

        spedUp = false;
    }
}
