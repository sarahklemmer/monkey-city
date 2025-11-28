using UnityEngine;
using UnityEngine.Assertions;

public class BuildingInfo : MonoBehaviour
{
    public static BuildingInfo instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BuildingInfo on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
    }

    public void PlayUpgradeEffect(BuildingBase building)
    {
        ParticleSystem upgradeEffect = building.GetComponentInChildren<ParticleSystem>();
        Assert.IsNotNull(upgradeEffect, "no particle system in children of buildinginfo on object " + gameObject.name);
        upgradeEffect.Play();
    }
}