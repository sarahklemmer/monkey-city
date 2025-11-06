using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlacementIndicatorOnClick : MonoBehaviour
{
    Building selectedBuilding;
    int grid_x;
    int grid_y;

    public void Initialize(Building building, int grid_x, int grid_y)
    {
        selectedBuilding = building;
        this.grid_x = grid_x;
        this.grid_y = grid_y;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.X)) BuildingGrid.instance.DestroyBuildingPlacementIndicators();
    }

    void OnMouseDown()
    {
        BuildingGrid.instance.Place(grid_x, grid_y, selectedBuilding);

        Vector3 pos = new Vector3(
            BuildingGrid.instance.GridXToWorldX(grid_x),
            1,
            BuildingGrid.instance.GridYToWorldZ(grid_y)
        );

        // the formula for centering is width or height - 1 * 0.5, so (n - 1)(0.5), if we didnt do this the center would be the grid coords rather
        // than being the lower left
        BuildingDimensions dim = BuildingUtils.TypeToDimensions(selectedBuilding.type);
        pos.x += (dim.width - 1) * 0.5f;
        pos.z += (dim.height - 1) * 0.5f;

        Instantiate(BuildingToPrefab.GetPrefab(selectedBuilding.type), pos, Quaternion.identity);
        // get rid of other placement indicators
        BuildingGrid.instance.DestroyBuildingPlacementIndicators();
    }
}
