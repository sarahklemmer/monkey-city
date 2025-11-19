using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    BuildingBase building;
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Click());
    }

    void Update()
    {
        button.interactable = !GlobalInteractionLock.IsLocked();
    }
    
    public void Click()
    {
        MoveButtonOnClick.ClickHandler(building);
    }
}
