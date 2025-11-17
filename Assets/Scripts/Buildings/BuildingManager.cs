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
            if (b.bananasPerDay != 0)
            {
                ShowFloatingText(b, b.bananasPerDay);
            }
        }
        int totalProduction = GetDailyProduction();
        Debug.Log($"Daily banana production: {totalProduction} bananas.");
        BananaManager.instance.AddBananas(totalProduction);
        
    }
    public void ShowFloatingText(BuildingBase building, int amount)
    {
        if (building == null || floatingTextPrefab == null) return;
        
        BuildingDimensions dims = BuildingUtils.TypeToDimensions(building.GetBuildingType());
        
        float heightOffset = dims.height + 1.5f; 
        
        Vector3 spawnPos = building.transform.position;
        spawnPos.y += heightOffset;
        
        GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
        BananaVisualization floater = textObj.GetComponent<BananaVisualization>();
        if (floater != null)
        {
            floater.Initialize(amount);
        }
    }

    public void ShowFloatingText(Vector3 position, int amount)
    {
        if (floatingTextPrefab == null) return;
        
        Vector3 spawnPos = position;
        spawnPos.y += 2f;
        
        GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
        BananaVisualization floater = textObj.GetComponent<BananaVisualization>();
        if (floater != null)
        {
            floater.Initialize(amount);
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
        Debug.LogError("error, trying to get tree of life when none exists");
        return null;
    }

    public int GetDailyProduction() => buildings.Sum(b => b.bananasPerDay);

    public List<BuildingBase> GetBuildingsOfType(BuildingType ty) => buildings.Where(b => b.GetBuildingType() == ty).ToList(); 

    // - 1 for TreeOfLife
    public int NumBuildings() => buildings.Count - 1;
    public int NumBuildings(BuildingType type) => buildings.Count(b => b.GetBuildingType() == type);
}
