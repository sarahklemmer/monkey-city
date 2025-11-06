using UnityEngine;

public class PlacementIndicatorOnClick : MonoBehaviour
{
    BuildingType buildingType;
    int grid_x;
    int grid_y;

    public void Initialize(Building building, int grid_x, int grid_y)
    {
        this.buildingType = building.type;
        this.grid_x = grid_x;
        this.grid_y = grid_y;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.X)) BuildingGrid.instance.DestroyBuildingPlacementIndicators();
    }

    void OnMouseDown()
    {
        Building placedBuilding = new Building(buildingType);
        
        BuildingGrid.instance.Place(grid_x, grid_y, placedBuilding);

        Vector3 pos = new Vector3(
            BuildingGrid.instance.GridXToWorldX(grid_x),
            1,
            BuildingGrid.instance.GridYToWorldZ(grid_y)
        );

        BuildingDimensions dim = BuildingUtils.TypeToDimensions(buildingType);
        pos.x += (dim.width - 1) * 0.5f;
        pos.z += (dim.height - 1) * 0.5f;

        GameObject buildingObj = Instantiate(BuildingToPrefab.GetPrefab(buildingType), pos, Quaternion.identity);
        
        placedBuilding.SetInstance(buildingObj);
        
        if (buildingObj.GetComponent<BuildingHealth>() == null)
        {
            buildingObj.AddComponent<BuildingHealth>();
        }

        BuildingGrid.instance.DestroyBuildingPlacementIndicators();
    }
}