using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public class BuildingTypeToPrice
{
    static float costScaleFactor = 0.5f;
    private static Dictionary<BuildingType, int> basePrice = new()
    {
        { BuildingType.TreeOfLife, 0 },
        { BuildingType.BananaFarm, 1 },
        { BuildingType.ArcherTower, 4 },
        { BuildingType.Wall, 2 }
    };

    public static int GetPrice(BuildingType ty)
    {
        Assert.IsTrue(
            System.Enum.GetValues(typeof(BuildingType)).Length == basePrice.Count &&
            System.Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().All(bt => basePrice.ContainsKey(bt)),
            "basePrice dict isn't one to one with the BuildingType enum!"
        );
        // base price + (scale factor * number of buildings )
        return basePrice[ty];
    }
}
