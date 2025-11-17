using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BananaProductionTimer : MonoBehaviour
{
    public static BananaProductionTimer instance;

    [SerializeField] private float productionInterval = 10f;

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
        if (BuildingManager.instance == null)
        {
            Debug.LogWarning("BananaProductionTimer: BuildingManager instance not found!");
            return;
        }

        List<BuildingBase> bananaFarms = BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm);
        
        if (bananaFarms == null || bananaFarms.Count == 0)
        {
            Debug.LogWarning("BananaProductionTimer: No banana farms found in scene!");
            return;
        }

        Debug.Log($"BananaProductionTimer: Found {bananaFarms.Count} banana farm(s), producing bananas...");

        int farmsProcessed = 0;
        foreach (BuildingBase building in bananaFarms)
        {
            if (building != null && building is BananaFarm)
            {
                BananaFarm farm = building as BananaFarm;
                farm.ProduceBananas();
                farmsProcessed++;
            }
        }
        
        Debug.Log($"BananaProductionTimer: Processed {farmsProcessed} banana farm(s)");
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

