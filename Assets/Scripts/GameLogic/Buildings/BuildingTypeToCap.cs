using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

public class BuildingCapToPrice
{
    private static Dictionary<BuildingType, int> count = new()
    {
        { BuildingType.TreeOfLife, 1 },
        { BuildingType.BananaFarm, 5 },
        { BuildingType.ArcherTower, 5 },
        { BuildingType.SpikeTrap, 1},
        { BuildingType.Wall, 0 },
        { BuildingType.Beacon, 5 }
    };

    public static int GetCap(BuildingType ty)
    {
        Assert.IsTrue(
            System.Enum.GetValues(typeof(BuildingType)).Length == count.Count &&
            System.Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().All(bt => count.ContainsKey(bt)),
            "count dict isn't one to one with the BuildingType enum!"
        );
        return count[ty];
    }
}