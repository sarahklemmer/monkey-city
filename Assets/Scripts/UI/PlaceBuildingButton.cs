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
        if(GlobalInteractionLock.IsLocked()) return;

        if (BananaManager.instance.GetBananas() < BuildingTypeToPrice.GetPrice(type)) return;
        // can only place 5 of all types
        if (BuildingManager.instance.GetBuildingsOfType(type).Count >= 5) return;
        
        if (type == BuildingType.Wall)
        {
            BananaManager.instance.RemoveBananas(BuildingTypeToPrice.GetPrice(type));

            BuildingGrid.instance.RevealPresetWalls();
            BuildingGrid.instance.FinishLevel();

            BuildingMenuManager.instance.ForceCloseMenu();
            BuildingMenuManager.instance.UpdatePrices();

            PlacementManager.instance.ClearCurrentBuilding();
            return;
        }

        Building building = new Building(type);

        BuildingMenuManager.instance.ForceCloseMenu();
        PlacementManager.instance.SetCurrentBuilding(building);
    }
}