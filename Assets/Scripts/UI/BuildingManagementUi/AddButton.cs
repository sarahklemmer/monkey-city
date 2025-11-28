using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class AddButton : MonoBehaviour
{
    [HideInInspector] public BuildingBase building;
    [SerializeField] Button button;
    BuildingBase victim;

    void Start()
    {
        Assert.IsNotNull(button, "forgot to assign add button in the inspector!");
        button.onClick.AddListener(() => Click());
    }

    void Update()
    {
        victim = building is TreeOfLife ? 
            BuildingManager.instance.GetClosestBuildingWithMonkeys(transform.position) : 
            BuildingManager.instance.GetTreeOfLife();
            
        if(victim == null || building == null) {
            button.interactable = false;
            return;
        }
        // if there's no monkeys left in the treeoflife we need to pull from the other type of building
        if(victim.GetMonkeyCount() == 0)
        {
            if(building is BananaFarm) victim = BuildingManager.instance.GetClosestBuildingNotOfTypeWithMonkeys(transform.position, BuildingType.BananaFarm);
            else
            {
                victim = 
                    BuildingManager.instance.GetClosestBuildingOfTypeWithMonkeys(transform.position, BuildingType.BananaFarm) ?? 
                    BuildingManager.instance.GetClosestBuildingNotOfTypeWithMonkeys(transform.position, building.type);
            }
        }

        button.interactable = building.CanAllocate() && victim != null;
    }
    
    public void Click()
    {
        Assert.IsNotNull(victim, "somehow victim is none in click");
        if(victim == null) return;
        victim.NextMonkeyToRemove().StartWalkingToBuilding(building);
    }

    public void PermanentlyRemoveButton()
    {
        Destroy(button.gameObject);
        enabled = false;
    }
}
