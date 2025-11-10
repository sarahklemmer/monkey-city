using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager instance;
    
    private Building currentBuildingForPlacement;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate PlacementManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }
    
    public void SetCurrentBuilding(Building building)
    {
        currentBuildingForPlacement = building;
        BuildingType type = building.type;
        
        if (building != null && BuildingGrid.instance != null)
        {
            if (type == BuildingType.TreeOfLife)
            {
                BuildingGrid.instance.SpawnSingleBuildingPlacementIndicators(building);
            }
            else
            {
                if (Tutorial.instance.tutorialActive && type == BuildingType.BananaFarm)
                {
                    BuildingDimensions dimensions = BuildingUtils.TypeToDimensions(building.type);

                    int w = dimensions.width;
                    int h = dimensions.height;

                    int x = BuildingGrid.instance.GetGridSize() / 2 + w / 2;
                    int y = BuildingGrid.instance.GetGridSize() / 2 + h / 2;
                    x += 2;
                    y += 2;
                    BuildingGrid.instance.SpawnSingleBuildingPlacementIndicators(building, x, y);
                }
                else if (Tutorial.instance.tutorialActive && type == BuildingType.ArcherTower)
                {
                    BuildingGrid.instance.SpawnSingleBuildingPlacementIndicators(building, 1, 1);
                }
                else
                {
                    BuildingGrid.instance.SpawnBuildingPlacementIndicators(building);
                }
            }
        }
    }
    
    public Building GetCurrentBuilding()
    {
        return currentBuildingForPlacement;
    }
    
    public void RefreshPlacementIndicators()
    {
        if (currentBuildingForPlacement != null && BuildingGrid.instance != null)
        {
            BuildingGrid.instance.DestroyBuildingPlacementIndicators();
            BuildingGrid.instance.SpawnBuildingPlacementIndicators(currentBuildingForPlacement);
        }
    }
    
    public void ClearCurrentBuilding()
    {
        currentBuildingForPlacement = null;
        if (BuildingGrid.instance != null)
        {
            BuildingGrid.instance.DestroyBuildingPlacementIndicators();
        }
    }
}