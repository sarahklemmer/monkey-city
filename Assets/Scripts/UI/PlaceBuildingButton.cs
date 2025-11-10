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
        if (Tutorial.instance.tutorialActive)
        {
            switch(type)
            {
                case BuildingType.TreeOfLife:
                    if (Tutorial.instance.tutorialStage != 1) return;
                    else Tutorial.instance.PlayerClickedTreeOfLifeButton();
                    break;
                case BuildingType.BananaFarm:
                    if (Tutorial.instance.tutorialStage != 3) return;
                    else Tutorial.instance.PlayerClickedBananaFarmButton();
                    break;
                case BuildingType.ArcherTower:
                    if (Tutorial.instance.tutorialStage != 9) return;
                    break;
            }
        } //1, 3, 9 are placing tree of life, placing farm, and archer tower respectively

        Building building = new Building(type);

        BuildingMenuManager.instance.ForceCloseBananaMenu();
        PlacementManager.instance.SetCurrentBuilding(building);
    }
}