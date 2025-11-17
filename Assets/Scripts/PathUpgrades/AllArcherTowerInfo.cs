using UnityEngine;

public class AllArcherTowerInfo : MonoBehaviour
{
    public static AllArcherTowerInfo instance;

    [SerializeField] private int baseDamagePerAttack = 10;

    [SerializeField] private float costMultiplier = 1.0f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate AllArcherTowerInfo on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public int GetDamagePerAttack()
    {
        return baseDamagePerAttack;
    }

    public void IncreaseDamagePerAttack(int value)
    {
        baseDamagePerAttack += value;
        Debug.Log($"Archer Tower damage increased to {baseDamagePerAttack}!");
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

