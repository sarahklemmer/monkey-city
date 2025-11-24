using UnityEngine;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
    [SerializeField] Sprite onImage;
    [SerializeField] Sprite offImage;
    Image thisImage;
    public static VolumeButton instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        thisImage = GetComponent<Image>();
    }

    public void ToggleVolume()
    {
        thisImage.sprite = Soundtrack.instance.Playing() ? offImage : onImage;
    }
}
