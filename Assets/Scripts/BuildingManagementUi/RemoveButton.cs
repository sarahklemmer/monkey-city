using UnityEngine;
using UnityEngine.UI;

public class RemoveButton : MonoBehaviour
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
        button.interactable = 
            !GlobalInteractionLock.IsLocked() &&
            building.NextMonkeyToRemove() != null;
    }
    
    public void Click()
    {
        // just go back to the tree of llife
        building.NextMonkeyToRemove().StartWalkingToBuilding(BuildingManager.instance.GetTreeOfLife());
    }
}
