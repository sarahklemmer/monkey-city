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

        if (BananaManager.instance.GetBananas() < BuildingTypeToPrice.GetPrice(type)) return;
        // can only place 5 of all types
        if (BuildingManager.instance.GetBuildingsOfType(type).Count >= 5) return;
        
        if (type == BuildingType.Wall)
        {
            BananaManager.instance.RemoveBananas(BuildingTypeToPrice.GetPrice(type));

            BuildingGrid.instance.RevealPresetWalls();
            BuildingGrid.instance.FinishLevel();

            PlacementManager.instance.ClearCurrentBuilding();
            return;
        }

        PlacementManager.instance.SetCurrentBuilding(type);
    }
}