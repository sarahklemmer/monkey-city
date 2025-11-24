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
        victim = BuildingManager.instance.GetTreeOfLife();
        if(victim == null) return;
        // if there's no monkeys left in the treeoflife we need to pull from the other type of building
        if(victim.GetMonkeyCount() == 0)
        {
            // if we're a banana farm pull from archer tower and vice versa
            BuildingType type = building.GetBuildingType() == BuildingType.BananaFarm ? BuildingType.ArcherTower : BuildingType.BananaFarm; 
            victim = BuildingManager.instance.GetClosestBuildingOfTypeWithMonkeys(building.transform.position, type);
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
