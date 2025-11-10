using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections;

public class BuildingGrid : MonoBehaviour
{
    public static BuildingGrid instance;

    
    [SerializeField] GameObject placementIndicatorPrefab;
    [SerializeField] Transform placementIndicatorsParent;
    [SerializeField] TMP_Text xToolTip;
    [SerializeField] GameObject presetWalls;
    [SerializeField] private string wallCompleteToast = "Congratulations! You beat this level of gameplay! Come back soon for more...";
    [SerializeField] private float wallCompleteToastDuration = 60f;
    [SerializeField] private string restartToast = "Restarting game in 5 seconds...";
    [SerializeField] private float restartToastDuration = 5f;
    private bool presetWallsRevealed = false;
    private bool levelCompleted = false;

    [SerializeField] int GRID_SIZE = 20;
    const float BASE_PLANE_SIZE = 10f;
    private Building[,] grid;
    private bool treeOfLifePlaced = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate grid on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;

        grid = new Building[GRID_SIZE, GRID_SIZE];
        transform.localScale = new Vector3(GRID_SIZE / BASE_PLANE_SIZE, 1, GRID_SIZE / BASE_PLANE_SIZE);
        // move indicators parent to the center of the grid
        placementIndicatorsParent.transform.position = transform.position;
    }

    void Start()
    {
        FrameCameraIsoTopBottom();
    }

    public void RevealPresetWalls()
    {
        if (presetWalls != null)
        {
            presetWalls.SetActive(true);
            presetWallsRevealed = true;
        }
        else
        {
            Debug.LogWarning("BuildingGrid presetWalls reference not assigned.");
        }
    }

    public int GetGridSize()
    {
        return GRID_SIZE;
    }

    public float GridXToWorldX(int grid_x)
    {
        return grid_x - (GRID_SIZE / 2);
    }

    public float GridYToWorldZ(int grid_y)
    {
        return grid_y - (GRID_SIZE / 2);
    }
    
    public float WorldY()
    {
        return transform.position.y;
    }   


    // starts at lower left corner of building
    public bool CanPlace(int x_start, int y_start, Building building)
    {
        int x_end = BuildingUtils.TypeToDimensions(building.type).width + x_start;
        int y_end = BuildingUtils.TypeToDimensions(building.type).height + y_start;

        Assert.IsFalse(x_end > GRID_SIZE, "placing building out of bounds");
        Assert.IsTrue(x_start >= 0, "placing building out of bounds");
        Assert.IsFalse(y_end > GRID_SIZE, "placing building out of bounds");
        Assert.IsTrue(y_start >= 0, "placing building out of bounds");

        if (x_start < 0 || y_start < 0 || x_end > GRID_SIZE || y_end > GRID_SIZE) return false;

        for (int x = x_start; x < x_end; x++)
        {
            for (int y = y_start; y < y_end; y++)
            {
                if (grid[x, y] is not null) return false;
            }
        }
        return true;
    }

    // starts at lower left corner of building
    public void Place(int x_start, int y_start, Building building)
    {
        Assert.IsTrue(CanPlace(x_start, y_start, building), "Trying to place building that can't be placed!");

        if (building.type == BuildingType.TreeOfLife) treeOfLifePlaced = true;

        int x_end = BuildingUtils.TypeToDimensions(building.type).width + x_start;
        int y_end = BuildingUtils.TypeToDimensions(building.type).height + y_start;

        for (int x = x_start; x < x_end; x++)
        {
            for (int y = y_start; y < y_end; y++)
            {
                grid[x, y] = building;
            }
        }
    }

    // From clickBuilding: Remove building from grid by Building reference
    public void RemoveBuilding(Building building)
    {
        if (building == null) return;

        // Find and clear all cells occupied by this building
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (grid[x, y] == building)
                {
                    grid[x, y] = null;
                }
            }
        }

        Debug.Log($"Building removed from grid. Cells now available for placement.");
    }

    // From clickBuilding: Remove building from grid by GameObject reference
    public void RemoveBuildingByGameObject(GameObject buildingGameObject)
    {
        if (buildingGameObject == null) return;

        // Find the Building object that references this GameObject
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                if (grid[x, y] != null && grid[x, y].instance == buildingGameObject)
                {
                    // Found it! Now remove all cells with this Building reference
                    Building buildingToRemove = grid[x, y];
                    RemoveBuilding(buildingToRemove);
                    Debug.Log($"Building removed from grid at ({x}, {y}). Cells now available for placement.");
                    return;
                }
            }
        }

        Debug.LogWarning($"Could not find building {buildingGameObject.name} in grid!");
    }

    // NEW: Clear building from grid using BuildingBase component
    public void ClearBuildingFromGrid(BuildingBase buildingBase)
    {
        if (buildingBase == null) return;
        
        // Use the existing method that works with GameObjects
        RemoveBuildingByGameObject(buildingBase.gameObject);
    }

    // don't worry about how this works, it works
    public void FrameCameraIsoTopBottom()
    {
        var cam = Camera.main;
        Assert.IsNotNull(cam, "Camera.main not found!");
        cam.orthographic = true;

        Vector3 center = transform.position;
        cam.transform.rotation = Quaternion.Euler(30f, 45f, 0f);
        float dist = GRID_SIZE * 2f;
        cam.transform.position = center - cam.transform.forward * dist;
        cam.transform.LookAt(center);

        float half = GRID_SIZE * 0.5f;
        Vector3[] corners =
        {
            center + new Vector3(-half, 0f, -half),
            center + new Vector3( half, 0f, -half),
            center + new Vector3( half, 0f,  half),
            center + new Vector3(-half, 0f,  half),
        };

        Vector3 up = cam.transform.up;
        float minV = float.PositiveInfinity, maxV = float.NegativeInfinity;
        foreach (var p in corners)
        {
            float v = Vector3.Dot(up, p - center);
            if (v < minV) minV = v;
            if (v > maxV) maxV = v;
        }

        // 1.4f just makes it look better trust
        cam.orthographicSize = ((maxV - minV) * 0.5f + 0.25f) * 1.4f;
    }

    //NOT optimal, use sliding window or smth
    private bool AreaFree(int x0, int y0, int w, int h)
    {
        if (x0 < 0 || y0 < 0 || x0 + w > GRID_SIZE || y0 + h > GRID_SIZE) return false;

        for (int x = x0; x < x0 + w; x++)
            for (int y = y0; y < y0 + h; y++)
                if (grid[x, y] is not null) return false;

        return true;
    }

    public void SpawnBuildingPlacementIndicators(Building building)
    {
        Assert.IsNotNull(placementIndicatorPrefab, "Assign a placementIndicatorPrefab in the Inspector!");
        if (!treeOfLifePlaced) return;

        xToolTip.enabled = true;
        BuildingManager.instance.MakeBuildingsTransparent();

        if (placementIndicatorsParent.childCount != 0)
        {
            DestroyBuildingPlacementIndicators();
        }

        BuildingDimensions dimensions = BuildingUtils.TypeToDimensions(building.type);

        int w = dimensions.width;
        int h = dimensions.height;

        for (int x = 0; x <= GRID_SIZE - w; x++)
        {
            for (int y = 0; y <= GRID_SIZE - h; y++)
            {
                if (!AreaFree(x, y, w, h)) continue;
                
                // annoyingly, y iz z, grid_size / 2 is because 0, 0 is the bottom left not the middle
                Vector3 pos = new Vector3(GridXToWorldX(x), 0, GridYToWorldZ(y));
                GameObject ind = Instantiate(placementIndicatorPrefab, pos, Quaternion.identity, placementIndicatorsParent);
                ind.GetComponent<PlacementIndicatorOnClick>().Initialize(building, x, y);
            }
        }
    }
    
    public void DestroyBuildingPlacementIndicators()
    {
        xToolTip.enabled = false;
        foreach (Transform child in placementIndicatorsParent) Destroy(child.gameObject);
    }

    public void SpawnSingleBuildingPlacementIndicators(Building building)
    {
        BuildingDimensions dimensions = BuildingUtils.TypeToDimensions(building.type);

        int w = dimensions.width;
        int h = dimensions.height;

        int x = GRID_SIZE / 2 - w / 2;
        int y = GRID_SIZE / 2 - h / 2;

        SpawnSingleBuildingPlacementIndicators(building, x, y);
    }
    
    public void SpawnSingleBuildingPlacementIndicators(Building building, int x, int y)
    {
        BuildingDimensions dimensions = BuildingUtils.TypeToDimensions(building.type);

        int w = dimensions.width;
        int h = dimensions.height;
        
        Assert.IsNotNull(placementIndicatorPrefab, "Assign a placementIndicatorPrefab in the Inspector!");

        if (placementIndicatorsParent.childCount != 0)
        {
            DestroyBuildingPlacementIndicators();
        }

        if (!AreaFree(x, y, w, h)) return;

        Vector3 pos = new Vector3(GridXToWorldX(x), 0, GridYToWorldZ(y));

        GameObject ind = Instantiate(placementIndicatorPrefab, pos, Quaternion.identity, placementIndicatorsParent);
        ind.GetComponent<PlacementIndicatorOnClick>().Initialize(building, x, y, building.type == BuildingType.TreeOfLife);
    }

    public void FinishLevel()
    {
        if (presetWallsRevealed && !levelCompleted)
        {
            levelCompleted = true;
            ToastManager.Instance.RequestToast(wallCompleteToast, wallCompleteToastDuration);
            ToastManager.Instance.RequestToast(restartToast, restartToastDuration);
            StartCoroutine(RestartSceneAfterDelay(wallCompleteToastDuration + restartToastDuration));
        }
    }

    private IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (SceneLoader.instance != null)
        {
            SceneLoader.instance.ReloadScene();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}