using UnityEngine;

public class AllBananaFarmInfo : MonoBehaviour
{
    public static AllBananaFarmInfo instance;

    [SerializeField] private int baseBananasPerDay = 1;

    [SerializeField] private float costMultiplier = 1.0f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate AllBananaFarmInfo on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public int GetBananasPerDay()
    {
        return baseBananasPerDay;
    }

    public void IncreaseBananasPerDay(int value)
    {
        baseBananasPerDay += value;
    }

    public float GetCostMultiplier()
    {
        return costMultiplier;
    }

    public void SetCostMultiplier(float value)
    {
        costMultiplier = Mathf.Max(0f, value);
    }

    // Helper method to get the modified cost (base cost * multiplier)
    public int GetModifiedCost(int baseCost)
    {
        return Mathf.RoundToInt(baseCost * costMultiplier);
    }
}

