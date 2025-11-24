using UnityEngine;

public class BuildingProgression : MonoBehaviour
{
    void Update()
    {   
        // unlock traps if we've placed a bananafarm and archertower
        if(
            BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm).Count > 0 && 
            BuildingManager.instance.GetBuildingsOfType(BuildingType.ArcherTower).Count > 0 &&
            !BuildingUnlock.Unlocked(BuildingType.SpikeTrap)
            )
        {
            BuildingUnlock.Unlock(BuildingType.SpikeTrap);
        }
    }
}
