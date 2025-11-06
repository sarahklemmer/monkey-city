using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    protected float health;
    public abstract void OnDayCycle();
    public abstract void OnDestroy();
}
