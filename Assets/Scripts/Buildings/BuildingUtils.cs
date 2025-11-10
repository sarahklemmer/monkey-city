using System.Collections.Generic;
using UnityEngine;

public enum BuildingType
{
    TreeOfLife,
    BananaFarm,
    ArcherTower,
    Wall,
}

public struct BuildingDimensions
{
    public readonly int width;
    public readonly int height;

    public BuildingDimensions(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}

public class BuildingUtils
{
    private static Dictionary<BuildingType, BuildingDimensions> typeToDimensions = new()
    {
        { BuildingType.TreeOfLife, new BuildingDimensions(4, 4) },
        { BuildingType.BananaFarm, new BuildingDimensions(1, 1) },
        { BuildingType.ArcherTower, new BuildingDimensions(1, 1) },
        { BuildingType.Wall, new BuildingDimensions(1, 1) }
    };

    public static BuildingDimensions TypeToDimensions(BuildingType type)
    {
        return typeToDimensions[type];
    }
}

public class Building
{
    public readonly BuildingType type;
    public GameObject instance;
    public BuildingHealth health;

    public Building(BuildingType in_type)
    {
        type = in_type;
    }

    public void SetInstance(GameObject obj)
    {
        instance = obj;
        health = obj.GetComponent<BuildingHealth>();
        
        if (health == null)
        {
            Debug.LogWarning($"Building of type {type} does not have BuildingHealth component!");
        }
    }

    public bool IsDestroyed()
    {
        return instance == null || (health != null && health.GetCurrentHealth() <= 0);
    }
}