using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

public class BuildingTypeToPrice
{
    private static Dictionary<BuildingType, int> basePrice = new()
    {
        { BuildingType.TreeOfLife, 0 },
        { BuildingType.BananaFarm, 1 },
        { BuildingType.ArcherTower, 4 },
        { BuildingType.SpikeTrap, 8},
        { BuildingType.Wall, 2 },
        { BuildingType.Beacon, 10 }
    };

    public static int GetPrice(BuildingType ty)
    {
        Assert.IsTrue(
            System.Enum.GetValues(typeof(BuildingType)).Length == basePrice.Count &&
            System.Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().All(bt => basePrice.ContainsKey(bt)),
            "basePrice dict isn't one to one with the BuildingType enum!"
        );
        return basePrice[ty];
    }
}