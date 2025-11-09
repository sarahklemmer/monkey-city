using UnityEngine;

public class PlacementIndicatorOnClick : MonoBehaviour
{
    BuildingType buildingType;
    int grid_x;
    int grid_y;
    bool isTreeOfLifeIndicator = false;

    public void Initialize(Building building, int grid_x, int grid_y, bool isTreeOfLifeIndicator = false)
    {
        buildingType = building.type;
        this.grid_x = grid_x;
        this.grid_y = grid_y;
        this.isTreeOfLifeIndicator = isTreeOfLifeIndicator;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.X) && !isTreeOfLifeIndicator)
        {
            ReturnToNormalState();
            
            if (PlacementManager.instance != null)
            {
                PlacementManager.instance.ClearCurrentBuilding();
            }
        }
    }

    void OnMouseDown()
    {
        try
        {
            Building placedBuilding = new Building(buildingType);

            BuildingGrid.instance.Place(grid_x, grid_y, placedBuilding);

            GameObject prefab = BuildingToPrefab.GetPrefab(buildingType);

            Vector3 pos = new Vector3(
                BuildingGrid.instance.GridXToWorldX(grid_x) + prefab.transform.position.x,
                prefab.transform.position.y,
                BuildingGrid.instance.GridYToWorldZ(grid_y) + prefab.transform.position.z
            );

            BuildingDimensions dim = BuildingUtils.TypeToDimensions(buildingType);

            pos.x += (dim.width - 1) * 0.5f;
            pos.z += (dim.height - 1) * 0.5f;

            if (buildingType != BuildingType.TreeOfLife)
            {
                BananaManager.instance.RemoveBananas(BuildingTypeToPrice.GetPrice(buildingType));
            }

            GameObject buildingObj = Instantiate(prefab, pos, prefab.transform.rotation);

            placedBuilding.SetInstance(buildingObj);

            // TODO: incorporate BuildingHealth into BuildingBase, this is here for now
            if (buildingObj.GetComponent<BuildingHealth>() == null)
            {
                buildingObj.AddComponent<BuildingHealth>();
            }

            if (buildingType == BuildingType.TreeOfLife)
            {
                if (PopulationManager.instance != null)
                {
                    PopulationManager.instance.AddToPopulation(5);
                }
            }
            else
            {
                BuildingMenuManager.instance.UpdatePrices();
            }

            if (PlacementManager.instance != null)
            {
                PlacementManager.instance.RefreshPlacementIndicators();
            }
        }
        finally
        {
            ReturnToNormalState();
        }
    }
    
    void ReturnToNormalState()
    {
        BuildingManager.instance.MakeBuildingsOpaque();
        BuildingGrid.instance.DestroyBuildingPlacementIndicators();
    }
}