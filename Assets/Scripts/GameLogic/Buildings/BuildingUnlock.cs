using System.Collections.Generic;

public class BuildingUnlock
{
    static HashSet<BuildingType> unlocked = new HashSet<BuildingType> { BuildingType.TreeOfLife };

    public static HashSet<BuildingType> GetUnlockedBuildings() => unlocked;
    public static bool Unlocked(BuildingType type) => unlocked.Contains(type);
    
    public static void Unlock(BuildingType type) 
    {
        if (unlocked.Contains(type)) return;
        unlocked.Add(type);
        ToastManager.Instance.RequestToast(type.ToString() + " unlocked!");
        ToggleFlasher.instance.StartFlash();
        
        if (BuildingMenuManager.instance != null)
        {
            BuildingMenuManager.instance.RefreshMenu();
        }
        
        if ((type == BuildingType.BananaFarm || type == BuildingType.ArcherTower) &&
            unlocked.Contains(BuildingType.BananaFarm) && 
            unlocked.Contains(BuildingType.ArcherTower) &&
            !unlocked.Contains(BuildingType.Beacon))
        {
            Unlock(BuildingType.Beacon);
        }
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