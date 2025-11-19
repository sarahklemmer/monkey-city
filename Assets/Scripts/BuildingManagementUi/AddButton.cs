using UnityEngine;
using UnityEngine.UI;

public class AddButton : MonoBehaviour
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
        // just for future proofing if you can put all monkeys in a building make it so that getmonkeycount is 
        // less than the total population
        button.interactable = 
            !GlobalInteractionLock.IsLocked() && 
            building.CanAllocate() && 
            building.GetMonkeyCount() < PopulationManager.instance.population;
    }
    
    public void Click()
    {
        // if we're a banana farm pull from archer tower and vice
        BuildingType type = building.GetBuildingType() == BuildingType.BananaFarm ? BuildingType.ArcherTower : BuildingType.BananaFarm; 
        BuildingBase closest = BuildingManager.instance.GetClosestBuildingOfTypeWithMonkeys(building.transform.position, type);
        closest.NextMonkeyToRemove().StartWalkingToBuilding(building);
    }
}
