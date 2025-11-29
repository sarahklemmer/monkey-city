using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BananaProductionTimer : MonoBehaviour
{
    public static BananaProductionTimer instance;

    public float productionInterval {get; private set; } = 5f;

    private bool isRunning = true;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BananaProductionTimer on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        Debug.Log($"BananaProductionTimer started! Production interval: {productionInterval} seconds");
        StartCoroutine(ProductionLoop());
    }

    IEnumerator ProductionLoop()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(productionInterval);
            
            if (isRunning)
            {
                Debug.Log($"BananaProductionTimer: Triggering production cycle...");
                ProduceBananasFromAllFarms();
            }
        }
    }

    void ProduceBananasFromAllFarms()
    {
        List<BuildingBase> bananaFarms = BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm);

        foreach (BuildingBase building in bananaFarms)
        {
            if (building != null && building is BananaFarm)
            {
                BananaFarm farm = building as BananaFarm;
                farm.ProduceBananas();
            }
        }
    }

    public void SetProductionInterval(float interval)
    {
        if (interval <= 0f)
        {
            Debug.LogWarning("Production interval must be greater than 0!");
            return;
        }

        productionInterval = interval;
        StopAllCoroutines();
        StartCoroutine(ProductionLoop());
    }

    public float GetProductionInterval() => productionInterval;

    public void StartProduction()
    {
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(ProductionLoop());
        }
    }

    public void StopProduction()
    {
        isRunning = false;
        StopAllCoroutines();
    }

    public bool IsRunning() => isRunning;
}

