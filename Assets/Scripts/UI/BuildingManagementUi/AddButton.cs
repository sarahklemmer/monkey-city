using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class AddButton : MonoBehaviour
{
    [HideInInspector] public BuildingBase building;
    [SerializeField] Button button;

    void Start()
    {
        Assert.IsNotNull(button, "forgot to assign add button in the inspector!");
        button.onClick.AddListener(() => Click());
    }

    void Update()
    {
        // just for future proofing if you can put all monkeys in a building make it so that getmonkeycount is 
        // less than the total population
        button.interactable = 
            building.CanAllocate() && 
            building.GetMonkeyCount() < PopulationManager.instance.population;
    }
    
    public void Click()
    {
        BuildingBase victim = BuildingManager.instance.GetTreeOfLife();
        // if there's no monkeys left in the treeoflife we need to pull from the other type of building
        if(victim.GetMonkeyCount() == 0)
        {
            // if we're a banana farm pull from archer tower and vice versa
            BuildingType type = building.GetBuildingType() == BuildingType.BananaFarm ? BuildingType.ArcherTower : BuildingType.BananaFarm; 
            victim = BuildingManager.instance.GetClosestBuildingOfTypeWithMonkeys(building.transform.position, type);
        }

        victim.NextMonkeyToRemove().StartWalkingToBuilding(building);
    }
}
