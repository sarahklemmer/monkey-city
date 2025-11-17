using UnityEngine;

public class MoveButtonOnClick : MonoBehaviour
{
    public static void ClickHandler(BuildingBase building)
    {
        building.SetVisible(false);
        BuildingGrid.instance.SpawnBuildingPlacementIndicators(building.GetInternalBuilding(), building);
    }
}
