using System.Collections.Generic;
using UnityEngine;

public class BuildingUnlock : MonoBehaviour
{
    static HashSet<BuildingType> unlocked = new HashSet<BuildingType> { BuildingType.TreeOfLife };
    
    private Dictionary<BuildingType, int> buildingsPlaced = new Dictionary<BuildingType, int>();
    
    public static BuildingUnlock instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckLibraryUnlock();
    }

    private void CheckLibraryUnlock()
    {
        if (unlocked.Contains(BuildingType.Library)) return;
        
        bool hasArcherTower = FindAnyObjectByType<ArcherTower>() != null;
        bool hasBananaFarm = FindAnyObjectByType<BananaFarm>() != null;

        if (hasArcherTower || hasBananaFarm)
        {
            Debug.Log($"Library unlock check: ArcherTower={hasArcherTower}, BananaFarm={hasBananaFarm}");
        }

        if (hasArcherTower && hasBananaFarm)
        {
            Debug.Log("Both buildings found! Unlocking Library...");
            Unlock(BuildingType.Library);
        }
    }

    public static HashSet<BuildingType> GetUnlockedBuildings() => unlocked;
    public static bool Unlocked(BuildingType type) => unlocked.Contains(type);
    
    public static void Unlock(BuildingType type) 
    {
        if (unlocked.Contains(type)) return;
        unlocked.Add(type);
        Debug.Log($"Building unlocked: {type}");
    }
    
    public static void Disable(BuildingType type) 
    {
        if (unlocked.Contains(type)) unlocked.Remove(type);
    }
    
    public static void Reset()
    {
        unlocked = new HashSet<BuildingType> { BuildingType.TreeOfLife };
        if (instance != null)
        {
            instance.buildingsPlaced.Clear();
        }
    }
    
    public void OnBuildingPlaced(BuildingType buildingType)
    {
        if (!buildingsPlaced.ContainsKey(buildingType))
        {
            buildingsPlaced[buildingType] = 0;
        }
        buildingsPlaced[buildingType]++;
        Debug.Log($"BuildingUnlock: {buildingType} placed. Count: {buildingsPlaced[buildingType]}/{GetMaxAllowed(buildingType)}");
    }
    
    public void OnBuildingDestroyed(BuildingType buildingType)
    {
        if (buildingsPlaced.ContainsKey(buildingType) && buildingsPlaced[buildingType] > 0)
        {
            buildingsPlaced[buildingType]--;
            
            if (BuildingManagementUi.instance != null)
            {
                BuildingManagementUi.instance.RequestUpdateUnlockedBuildings();
            }
        }
    }
    
    public bool CanPlaceBuilding(BuildingType buildingType)
    {
        if (!buildingsPlaced.ContainsKey(buildingType))
        {
            return true;
        }
        
        int maxAllowed = GetMaxAllowed(buildingType);
        if (maxAllowed == int.MaxValue)
        {
            return true;
        }
        
        return buildingsPlaced[buildingType] < maxAllowed;
    }
    
    public int GetPlacedCount(BuildingType buildingType)
    {
        if (!buildingsPlaced.ContainsKey(buildingType))
        {
            return 0;
        }
        return buildingsPlaced[buildingType];
    }

    private int GetMaxAllowed(BuildingType buildingType)
    {
        if (buildingType == BuildingType.Library)
        {
            return 1;
        }
        
        return int.MaxValue;
    }
}