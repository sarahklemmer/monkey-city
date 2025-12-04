using UnityEngine;

public class BuildingProgression : MonoBehaviour
{
    void Update()
    {   
        if(BuildingManager.instance.GetTreeOfLife() != null && !BuildingUnlock.Unlocked(BuildingType.BananaFarm))
        {
            BuildingUnlock.Unlock(BuildingType.BananaFarm);
            BuildingUnlock.Unlock(BuildingType.ArcherTower);
            BuildingUnlock.Unlock(BuildingType.Beacon);
            BuildingUnlock.Disable(BuildingType.TreeOfLife);
            BuildingManagementUi.instance.RequestUpdateUnlockedBuildings();
        }

        if(
            BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm).Count > 0 && 
            BuildingManager.instance.GetBuildingsOfType(BuildingType.ArcherTower).Count > 0 &&
            !BuildingUnlock.Unlocked(BuildingType.SpikeTrap)
        )
        {
            BuildingUnlock.Unlock(BuildingType.SpikeTrap);
            BuildingManagementUi.instance.RequestUpdateUnlockedBuildings();
        }

        if(
            BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm).Count > 0 && 
            BuildingManager.instance.GetBuildingsOfType(BuildingType.ArcherTower).Count > 0 &&
            !BuildingUnlock.Unlocked(BuildingType.Library)
        )
        {
            BuildingUnlock.Unlock(BuildingType.Library);
            BuildingManagementUi.instance.RequestUpdateUnlockedBuildings();
        }
    }
}