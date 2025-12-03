using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    [SerializeField] List<BuildingBase> buildings;
    [SerializeField] private GameObject floatingTextPrefab;

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
        return null;
    }

    public BuildingBase GetClosestBuildingOfTypeWithMonkeys(Vector3 position, BuildingType type)
    {    
        BuildingBase closest = null;
        float lowestDistance = float.MaxValue;
        foreach(BuildingBase building in buildings.Where(b => b.GetBuildingType() == type && b.GetMonkeyCount() > 0))
        {
            float dist = Vector3.Distance(position, building.transform.position);
            if(dist < lowestDistance)
            {
                lowestDistance = dist;
                closest = building;
            }
        }
        return closest;
    }

    // D:
    public BuildingBase GetClosestBuildingWithMonkeys(Vector3 position)
    {    
        BuildingBase closest = null;
        float lowestDistance = float.MaxValue;
        foreach(BuildingBase building in buildings.Where(b => b.GetMonkeyCount() > 0))
        {
            float dist = Vector3.Distance(position, building.transform.position);
            if(dist < lowestDistance)
            {
                lowestDistance = dist;
                closest = building;
            }
        }
        return closest;
    }


    public BuildingBase GetClosestBuildingNotOfTypeWithMonkeys(Vector3 position, BuildingType type)
    {    
        BuildingBase closest = null;
        float lowestDistance = float.MaxValue;
        foreach(BuildingBase building in buildings.Where(b => b.GetBuildingType() != type && b.GetMonkeyCount() > 0))
        {
            float dist = Vector3.Distance(position, building.transform.position);
            if(dist < lowestDistance)
            {
                lowestDistance = dist;
                closest = building;
            }
        }
        return closest;
    }

    public void HealAllToFull() {
        foreach(BuildingBase b in buildings) { if(b.health.GetCurrentHealth() > 0) b.health.HealToFull(); }
    }

    public int GetDailyProduction() => buildings.Sum(b => b.bananasPerDay);

    public int GetBananaProduction() => buildings.OfType<BananaFarm>().Sum(b => b.bananasToProduce);

    public List<BuildingBase> GetBuildingsOfType(BuildingType ty) => buildings.Where(b => b.GetBuildingType() == ty).ToList();

    // - 1 for TreeOfLife
    public int NumBuildings() => buildings.Count - 1;
    public int NumBuildings(BuildingType type) => buildings.Count(b => b.GetBuildingType() == type);
}
