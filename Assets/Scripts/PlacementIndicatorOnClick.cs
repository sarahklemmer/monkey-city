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
        // dont want to let the player quit out of placing the tutorial (tree of life) indicator accidentally
        if (Input.GetKey(KeyCode.X) && !isTreeOfLifeIndicator) ReturnToNormalState();
    }

    void OnMouseDown()
    {
        try
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

            // spend bananas BEFORE INSTANTIATING, as creating a new building will affect price
            if(buildingType != BuildingType.TreeOfLife) BananaManager.instance.RemoveBananas(BuildingTypeToPrice.GetPrice(buildingType));
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
                // if it's the tree of life we add 1 to the population and don't spend any money
                //TODO: make this like 5
                PopulationManager.instance.AddToPopulation(1);
            } else
            {
                // otherwise spend bananas
                BuildingMenuManager.instance.UpdatePrices();
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