using UnityEngine;
using UnityEngine.Assertions;

public class BuildingGrid : MonoBehaviour
{
    public static BuildingGrid instance;

    public const int GRID_SIZE = 200;
    private Building[,] grid = new Building[GRID_SIZE, GRID_SIZE];

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate grid on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public bool CanPlace(int x_start, int y_start, Building building)
    {
        int x_end = building.dimensions.width + x_start;
        int y_end = building.dimensions.height + y_start;

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

    public void Place(int x_start, int y_start, Building building)
    {
        Assert.IsTrue(CanPlace(x_start, y_start, building), "Trying to place building that can't be placed!");

        int x_end = building.dimensions.width + x_start;
        int y_end = building.dimensions.height + y_start;

        for (int x = x_start; x < x_end; x++)
        {
            for (int y = y_start; y < y_end; y++)
            {
                grid[x, y] = building;
            }
        }
    }
}

