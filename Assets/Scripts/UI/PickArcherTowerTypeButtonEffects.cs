using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class PickArcherTowerTypeButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject infoPopup;

    private Image icon;
    private Button b;
    private bool isHovering = false;

    void Awake()
    {
        icon = GetComponent<Image>();
        b = GetComponent<Button>();
    }

    void OnEnable()
    {
        SetInfoPopupActive(false);
    }

    void Update()
    {
        if (icon != null && b != null)
        {
            Color c = icon.color;
            c.a = b.interactable ? 1f : 0.2f;
            icon.color = c;
        }

        Debug.Log(b.interactable);

        SetInfoPopupActive(b != null && b.interactable && isHovering);
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
