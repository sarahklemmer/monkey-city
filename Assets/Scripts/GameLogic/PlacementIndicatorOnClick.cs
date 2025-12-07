using UnityEngine.Assertions;
using UnityEngine;
using UnityEngine.AI;

public class PlacementIndicatorOnClick : MonoBehaviour
{
    BuildingType type;
    BuildingBase existingBuilding;
    int grid_x;
    int grid_y;
    bool isTreeOfLifeIndicator = false;
    bool isMoving = false;

    private TutorialManager tutorialManager;

    public void Initialize(BuildingType type, int grid_x, int grid_y, bool isTreeOfLifeIndicator = false)
    {
        this.type = type;
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
        type = existingBuilding.GetBuildingType();
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
            GameObject prefab = BuildingToPrefab.GetPrefab(type);

            Vector3 pos = new Vector3(
                BuildingGrid.instance.GridXToWorldX(grid_x) + prefab.transform.position.x,
                isMoving ? existingBuilding.transform.position.y : prefab.transform.position.y,
                BuildingGrid.instance.GridYToWorldZ(grid_y) + prefab.transform.position.z
            );

            BuildingDimensions dim = BuildingUtils.TypeToDimensions(type);

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
                BuildingGrid.instance.Place(grid_x, grid_y, type);
                NotifyEnemiesOfBuildingMoved();
                return;
            }
            //TODO: ask kyle about this or make walls not movable
            if (type == BuildingType.Wall)
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

            BuildingGrid.instance.Place(grid_x, grid_y, type);
            GameObject buildingObj = Instantiate(prefab, pos, rotation);

            // set buildingbase health to either an existing buildinghealth or a fresh one that we add
            if (buildingObj.GetComponent<BuildingHealth>() == null)
            {
                buildingObj.GetComponent<BuildingBase>().health = buildingObj.AddComponent<BuildingHealth>();
            } else
            {
                buildingObj.GetComponent<BuildingBase>().health = buildingObj.GetComponent<BuildingHealth>();
            }

            buildingObj.GetComponent<BuildingBase>().SetGridCoords(grid_x, grid_y);

            // treeoflife is free
            BananaManager.instance.RemoveBananas(BuildingTypeToPrice.GetPrice(type));
            PlacementManager.instance.RefreshPlacementIndicators();
            BuildingHealth newBuildingHealth = buildingObj.GetComponent<BuildingHealth>();
            NotifyEnemiesOfNewBuilding(newBuildingHealth);
            if (tutorialManager == null)
            {
                tutorialManager = FindFirstObjectByType<TutorialManager>();
            }
            Debug.Log($"isActive: {tutorialManager.isActive}, currentStepIndex: {tutorialManager.currentStepIndex}");
            if (tutorialManager.isActive && tutorialManager.currentStepIndex == 1)
            {
                Debug.Log($"Tutorial active, completing step {tutorialManager.currentStepIndex}");
                tutorialManager.OnStepCompleted();
            }
            if (tutorialManager.isActive && tutorialManager.currentStepIndex == 3)
            {
                Debug.Log($"Tutorial active, completing step {tutorialManager.currentStepIndex}");
                tutorialManager.OnStepCompleted();
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