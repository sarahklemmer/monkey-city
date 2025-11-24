using UnityEngine.Assertions;
using UnityEngine;
using System.Linq;

public class PlacementIndicatorOnClick : MonoBehaviour
{
    BuildingType buildingType;
    BuildingBase existingBuilding;
    int grid_x;
    int grid_y;
    bool isTreeOfLifeIndicator = false;
    bool isMoving = false;

    public void Initialize(Building building, int grid_x, int grid_y, bool isTreeOfLifeIndicator = false)
    {
        buildingType = building.type;
        this.grid_x = grid_x;
        this.grid_y = grid_y;
        this.isTreeOfLifeIndicator = isTreeOfLifeIndicator;
    }

    public void InitializeWithExistingBuilding(BuildingBase existingBuilding, int grid_x, int grid_y)
    {
        this.grid_x = grid_x;
        this.grid_y = grid_y;
        isMoving = true;
        this.existingBuilding = existingBuilding;
        buildingType = existingBuilding.GetBuildingType();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.X) && !isTreeOfLifeIndicator && !isMoving)
        {
            ReturnToNormalState();
            PlacementManager.instance.ClearCurrentBuilding();
        }
    }

    void OnMouseDown()
    {
        try
        {
            // disable building selection for 1 frame
            BuildingSelector.instance.DisableSelectionThisFrame();

            Building placedBuilding = new Building(buildingType);

            GameObject prefab = BuildingToPrefab.GetPrefab(buildingType);

            Vector3 pos = new Vector3(
                BuildingGrid.instance.GridXToWorldX(grid_x) + prefab.transform.position.x,
                prefab.transform.position.y,
                BuildingGrid.instance.GridYToWorldZ(grid_y) + prefab.transform.position.z
            );

            BuildingDimensions dim = BuildingUtils.TypeToDimensions(buildingType);

            pos.x += (dim.width - 1) * 0.5f;
            pos.z += (dim.height - 1) * 0.5f;

            Quaternion rotation = prefab.transform.rotation;
            if (isMoving)
            {
                Assert.IsNotNull(existingBuilding, "existing building cannot be null if we're moving!");
                existingBuilding.transform.position = pos;
                existingBuilding.transform.rotation = rotation;
                existingBuilding.MoveMonkeysToPos(pos);
                existingBuilding.SetGridCoords(grid_x, grid_y);
                // remove and then immediately place in its new destination
                BuildingGrid.instance.Place(grid_x, grid_y, existingBuilding.GetInternalBuilding());
                NotifyEnemiesOfBuildingMoved();
                return;
            }
            //TODO: ask kyle about this or make walls not movable
            if (buildingType == BuildingType.Wall)
            {
                Assert.IsFalse(isMoving, "can't move walls until we talk to Kyle!");
                Vector3 center = BuildingGrid.instance.transform.position;
                Vector3 directionFromCenter = pos - center;
                Vector3 baseAngles = prefab.transform.rotation.eulerAngles;
                if (Mathf.Abs(directionFromCenter.x) > Mathf.Abs(directionFromCenter.z))
                {
                    rotation = Quaternion.Euler(baseAngles.x, baseAngles.y + 90f, baseAngles.z);
                }
                else
                {
                    rotation = Quaternion.Euler(baseAngles);
                }
            }

            BuildingGrid.instance.Place(grid_x, grid_y, placedBuilding);
            GameObject buildingObj = Instantiate(prefab, pos, rotation);
            buildingObj.GetComponent<BuildingBase>().SetGridCoords(grid_x, grid_y);

            // TODO: incorporate BuildingHealth into BuildingBase, this is here for now
            if (buildingObj.GetComponent<BuildingHealth>() == null)
            {
                buildingObj.AddComponent<BuildingHealth>();
            }

            placedBuilding.SetInstance(buildingObj);


            // spawn monkeys if treeoflife
            if (buildingType == BuildingType.TreeOfLife) PopulationManager.instance.AddToPopulation(1);
            // otherwise pay for building
            else BananaManager.instance.RemoveBananas(BuildingTypeToPrice.GetPrice(buildingType));
            
            BuildingMenuManager.instance.UpdatePrices();
            PlacementManager.instance.RefreshPlacementIndicators();
            BuildingHealth newBuildingHealth = buildingObj.GetComponent<BuildingHealth>();
            NotifyEnemiesOfNewBuilding(newBuildingHealth);


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
        // if we error and exit early want the building to come back
        if(existingBuilding != null) existingBuilding.SetVisible(true);
        if(existingBuilding != null) UIInteractabilityManager.instance.EnableInteractivity();
    }

    private void NotifyEnemiesOfNewBuilding(BuildingHealth newBuilding)
    {
        if (newBuilding == null) return;

        EnemyAttacker[] enemies = Object.FindObjectsByType<EnemyAttacker>(FindObjectsSortMode.None);
        if (enemies == null || enemies.Length == 0) return;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.UpdateTargetBuilding(newBuilding);
        }
    }

    private void NotifyEnemiesOfBuildingMoved()
    {
        EnemyAttacker[] enemies = Object.FindObjectsByType<EnemyAttacker>(FindObjectsSortMode.None);
        if (enemies == null || enemies.Length == 0) return;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;
            enemy.UpdateTargetBuilding();
        }
    }
}