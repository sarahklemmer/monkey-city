using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    protected Building building;

    public abstract void OnDayCycle();
    public abstract void OnDestroy();

    // basically RAII for the BuildingManager, super handy
    protected virtual void OnEnable()
    {
        BuildingManager.instance.AddBuilding(this);
    }

    protected virtual void OnDisable()
    {
        BuildingManager.instance.RemoveBuilding(this);
    }
}
