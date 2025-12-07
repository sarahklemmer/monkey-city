using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//NOTE!!!! do not put anything between bananafarm and archertower or it will break the building UI on the right!
public enum BuildingType
{
    TreeOfLife,
    BananaFarm,
    ArcherTower,
    SpikeTrap,
    Wall,
    Beacon,
    Library,
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
        { BuildingType.TreeOfLife, new BuildingDimensions(6, 6) },
        { BuildingType.BananaFarm, new BuildingDimensions(1, 1) },
        { BuildingType.ArcherTower, new BuildingDimensions(1, 1) },
        { BuildingType.SpikeTrap, new BuildingDimensions(1, 1) },
        { BuildingType.Wall, new BuildingDimensions(1, 1) },
        { BuildingType.Beacon, new BuildingDimensions(1, 1) },
        { BuildingType.Library, new BuildingDimensions(2, 2) }
    };

    public static BuildingDimensions TypeToDimensions(BuildingType type)
    {
        Assert.AreEqual(
            typeToDimensions.Count,
            System.Enum.GetValues(typeof(BuildingType)).Length,
            "TypeToDimensions doesn't align with number of building types, either extras are assigned in the editor or you forgot to add a mapping when you made a new type"
        );
        return typeToDimensions[type];
    }
}