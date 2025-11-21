using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class RemoveButton : MonoBehaviour
{
    [HideInInspector] public BuildingBase building;
    [SerializeField] Button button;

    void Start()
    {
        Assert.IsNotNull(button, "forgot to assign remove button in the inspector!");
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
