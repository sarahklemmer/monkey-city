using UnityEngine;

public class PlaceBuildingTesterDELETEME : MonoBehaviour
{
    void Start()
    {
        Building test = new Building(BuildingType.BananaFarm);
        BuildingGrid.instance.SpawnBuildingPlacementIndicators(test);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Building test = new Building(BuildingType.TreeOfLife);
            BuildingGrid.instance.SpawnBuildingPlacementIndicators(test);
        } else if(Input.GetKeyDown(KeyCode.K))
        {
            Building test = new Building(BuildingType.ArcherTower);
            BuildingGrid.instance.SpawnBuildingPlacementIndicators(test);
        }
    }
}
