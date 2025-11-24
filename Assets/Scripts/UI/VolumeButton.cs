using UnityEngine;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour
{
    [SerializeField] Sprite onImage;
    [SerializeField] Sprite offImage;
    Image thisImage;

    void Start()
    {
        thisImage = GetComponent<Image>();
    }

    void Update()
    {
        if(BuildingManager.instance.GetTreeOfLife() == null) return;
        // this isn't backwards, it's to toggle music on/off
        thisImage.sprite = Soundtrack.instance.Playing() ? offImage : onImage;
    }
}
