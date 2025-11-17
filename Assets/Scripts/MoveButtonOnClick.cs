using UnityEngine;

public class MoveButtonOnClick : MonoBehaviour
{
    public static void ClickHandler(BuildingBase building)
    {
        building.SetVisible(false);
        BuildingGrid.instance.RemoveBuilding(building);

        BuildingGrid.instance.SpawnBuildingPlacementIndicators(building.GetInternalBuilding(), building);
    }
}
