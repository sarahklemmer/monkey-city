using System.Collections.Generic;
using System.Diagnostics;

public class BuildingUnlock
{
    static HashSet<BuildingType> unlocked = new HashSet<BuildingType> { BuildingType.TreeOfLife };

    public static HashSet<BuildingType> GetUnlockedBuildings() => unlocked;
    public static bool Unlocked(BuildingType type) => unlocked.Contains(type);
    
    public static void Unlock(BuildingType type) 
    {
        if (unlocked.Contains(type)) return;
        unlocked.Add(type);
        ToastBuildingManager.Instance.RequestToast(type.ToString() + " unlocked!");
        Debug.WriteLine($"Building unlocked: {type}");
    }
    
    public static void Disable(BuildingType type) 
    {
        if (unlocked.Contains(type)) unlocked.Remove(type);
    }
    
    public static void Reset()
    {
        unlocked = new HashSet<BuildingType> { BuildingType.TreeOfLife };
    }
}