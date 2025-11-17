using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    [SerializeField] List<BuildingBase> buildings;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BuildingManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;

        buildings = new List<BuildingBase>();
    }

    public void RemoveBuilding(BuildingBase b) => buildings.Remove(b);


    public void AddBuilding(BuildingBase b) => buildings.Add(b);

    public void PassDay()
    {
        foreach (BuildingBase b in buildings)
        {
            b.OnDayCycle();
        }
        BananaManager.instance.AddBananas(GetDailyProduction());
    }

    public void MakeBuildingsTransparent()
    {
        BuildingSelector.instance.Deselect();
        foreach (BuildingBase b in buildings)
        {
            b.MakeTransparent();
        }
    }

    public void MakeBuildingsOpaque()
    {
        foreach (BuildingBase b in buildings)
        {
            b.MakeOpaque();
        }
    }

    public BuildingBase GetTreeOfLife()
    {
        foreach (BuildingBase b in buildings)
        {
            if (b is TreeOfLife) return b;
        }
        Debug.LogError("error, trying to get tree of life when none exists");
        return null;
    }

    public int GetDailyProduction() => buildings.Sum(b => b.bananasPerDay);

    public int GetBananaProduction() => buildings.OfType<BananaFarm>().Sum(b => b.bananasToProduce);

    public List<BuildingBase> GetBuildingsOfType(BuildingType ty) => buildings.Where(b => b.GetBuildingType() == ty).ToList();

    // - 1 for TreeOfLife
    public int NumBuildings() => buildings.Count - 1;
    public int NumBuildings(BuildingType type) => buildings.Count(b => b.GetBuildingType() == type);
}
