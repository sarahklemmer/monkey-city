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
        
        if (building != null && BuildingGrid.instance != null)
        {
            if (building.type == BuildingType.TreeOfLife)
            {
                BuildingGrid.instance.SpawnSingleBuildingPlacementIndicators(building);
            }
            else
            {
                BuildingGrid.instance.SpawnBuildingPlacementIndicators(building);
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