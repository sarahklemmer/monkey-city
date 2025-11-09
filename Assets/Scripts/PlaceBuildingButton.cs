using UnityEngine;
using UnityEngine.Assertions;

public class PlaceBuildingButton : MonoBehaviour
{
    BuildingType type;
    private bool initialized = false;

    public void Initialize(BuildingType type)
    {
        this.type = type;
        initialized = true;
    }

    public void OnClick()
    {
        Assert.IsTrue(initialized, "trying to click non initialized button");
        //TODO: add price checks
        
        Building building = new Building(type);
        
        // Set this as the current building in PlacementManager
        if (PlacementManager.instance != null)
        {
            PlacementManager.instance.SetCurrentBuilding(building);
        }
        else
        {
            // Fallback if PlacementManager doesn't exist yet
            // If Tree of Life, use single placement indicator (for tutorial)
            if (type == BuildingType.TreeOfLife)
            {
                BuildingGrid.instance.SpawnSingleBuildingPlacementIndicators(building);
            }
            else
            {
                BuildingGrid.instance.SpawnBuildingPlacementIndicators(building);
            }
        }
    }
}