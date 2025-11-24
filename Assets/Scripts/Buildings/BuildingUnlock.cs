using System.Collections.Generic;

public class BuildingUnlock
{
    static HashSet<BuildingType> unlocked = new HashSet<BuildingType> { BuildingType.TreeOfLife };

    public static HashSet<BuildingType> GetUnlockedBuildings() => unlocked;
    public static bool Unlocked(BuildingType type) => unlocked.Contains(type);
    public static void Unlock(BuildingType type) 
    {
        unlocked.Add(type);
        ToastManager.Instance.RequestToast(type.ToString() + " unlocked!");
        ToggleFlasher.instance.StartFlash();
    }
    public static void Disable(BuildingType type) 
    {
        if (unlocked.Contains(type)) unlocked.Remove(type);
    }

    public static void Reset()
    {
        unlocked.Clear();
        unlocked.Add(BuildingType.TreeOfLife);
    }
}
