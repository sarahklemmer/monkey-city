using UnityEngine;

public class MoveButtonOnClick : MonoBehaviour
{
    public void ClickHandler(BuildingBase building)
    {
        building.SetVisible(false);
        BuildingGrid.instance.SpawnBuildingPlacementIndicators(building.GetInternalBuilding(), building);
    }
}
