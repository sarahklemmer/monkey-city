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

        GameObject prefab = BuildingToPrefab.GetPrefab(buildingType);

        // calculate position for building model
        Vector3 pos = new Vector3(
            BuildingGrid.instance.GridXToWorldX(grid_x) + prefab.transform.position.x,
            prefab.transform.position.y,
            BuildingGrid.instance.GridYToWorldZ(grid_y) + prefab.transform.position.z
        );

        BuildingDimensions dim = BuildingUtils.TypeToDimensions(buildingType);

        // center the model (translating from lower-left based coordinate system to center based)
        pos.x += (dim.width - 1) * 0.5f;
        pos.z += (dim.height - 1) * 0.5f;

        // instantiate prefab
        GameObject buildingObj = Instantiate(prefab, pos, prefab.transform.rotation);

        placedBuilding.SetInstance(buildingObj);
        
        //TODO: incorporate BuildingHealth into BuildingBase, this is here for now
        if (buildingObj.GetComponent<BuildingHealth>() == null)
        {
            buildingObj.AddComponent<BuildingHealth>();
        }

        if (buildingType == BuildingType.TreeOfLife)
        {
            if (PopulationManager.instance != null)
            {
                PopulationManager.instance.AddToPopulation(1);
            }
        }

        BuildingGrid.instance.DestroyBuildingPlacementIndicators();
    }
}