using System.Collections.Generic;

public enum BuildingType
{
    TreeOfLife,
    BananaFarm,
    ArcherTower,
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
        { BuildingType.ArcherTower, new BuildingDimensions(1, 1) }
    };

    public static BuildingDimensions TypeToDimensions(BuildingType type)
    {
        return typeToDimensions[type];
    }
}

public class Building
{
    public readonly BuildingType type;

    public Building(BuildingType in_type)
    {
        type = in_type;
    }
}