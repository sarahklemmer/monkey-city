using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    Building building;
    public abstract void OnDayCycle();
    public abstract void OnDestroy();
}
