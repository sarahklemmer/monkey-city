using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    protected Building building;
    
    public abstract void OnDayCycle();
    public abstract void OnDestroy();

    protected virtual void SharedAwakeBehavior()
    {
    }

    public virtual void Remove()
    {
        if (BuildingGrid.instance != null)
        {
            BuildingGrid.instance.RemoveBuildingByGameObject(gameObject);
            
            if (PlacementManager.instance != null)
            {
                PlacementManager.instance.RefreshPlacementIndicators();
            }
        }
        
        if (BuildingInfo.instance != null)
        {
            BuildingInfo.instance.Hide();
        }
        
        Destroy(gameObject);
    }

    void OnMouseDown()
    {
        if (BuildingInfo.instance != null)
        {
            BuildingInfo.instance.Show(this);
        }
    }

    public virtual void EnableGlow()
    {
    }

    public virtual void DisableGlow()
    {
    }

    public virtual int GetMonkeyCount()
    {
        return 0;
    }

    public virtual int GetMonkeyCapacity()
    {
        return 0;
    }
}