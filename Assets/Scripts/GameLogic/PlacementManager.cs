using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager instance;
    public BuildingType? type { get; private set;}
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate PlacementManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }
    
    public void SetCurrentBuilding(BuildingType type)
    {
        this.type = type;
        
        if (type == BuildingType.TreeOfLife) BuildingGrid.instance.SpawnSingleBuildingPlacementIndicators(type);
        else BuildingGrid.instance.SpawnBuildingPlacementIndicators(type);
    }
    
    public void RefreshPlacementIndicators()
    {
        if (type != null)
        {
            BuildingGrid.instance.DestroyBuildingPlacementIndicators();
            BuildingGrid.instance.SpawnBuildingPlacementIndicators(type.Value);
        }
    }
    
    public void ClearCurrentBuilding()
    {
        type = null;
        if (BuildingGrid.instance != null)
        {
            BuildingGrid.instance.DestroyBuildingPlacementIndicators();
        }
    }
}