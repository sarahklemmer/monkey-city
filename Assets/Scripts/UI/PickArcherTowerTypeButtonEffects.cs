using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class PickArcherTowerTypeButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject infoPopup;

    private bool isHovering = false;

    void OnEnable()
    {
        SetInfoPopupActive(false);
    }

    void Update()
    {
        SetInfoPopupActive(isHovering);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    private void SetInfoPopupActive(bool active)
    {
        if (infoPopup != null && infoPopup.activeSelf != active)
        {
            infoPopup.SetActive(active);
        }
    }
}
