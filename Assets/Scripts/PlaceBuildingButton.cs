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
        BuildingGrid.instance.SpawnBuildingPlacementIndicators(new Building(type));
    }
}
 