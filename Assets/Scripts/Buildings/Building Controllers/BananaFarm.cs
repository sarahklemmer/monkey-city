using UnityEngine;

public class BananaFarm : BuildingBase
{
    private int level = 1;
    private const int MAX_LEVEL = 4;
    private int baseBananaProduction = 1;
    public int bananasToProduce = 0;
    public int buildingLevel = 1;
    private static readonly int[] upgradeCosts = { 0, 5, 40, 100, 200 };
    
    [SerializeField] ParticleSystem upgradeEffect;
    
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.BananaFarm);
        monkeys = new(2);
    }

    void Update()
    {
        if (AllBananaFarmInfo.instance != null && monkeys != null)
        {
            bananasToProduce = AllBananaFarmInfo.instance.GetBananasPerDay() * monkeys.Count() * buildingLevel;
        }
    }

    public override void OnDayCycle()
    {
        if (monkeys != null && monkeys.Count() > 0 && AllBananaFarmInfo.instance != null)
        {
            BananaManager.instance.AddBananas(AllBananaFarmInfo.instance.GetBananasPerDay() * monkeys.Count());
        }
    }

    public void ProduceBananas()
    {   
        if (BananaManager.instance != null)
        {
            BananaManager.instance.AddBananas(bananasToProduce);
            Debug.Log($"BananaFarm: Produced {bananasToProduce} bananas ({baseBananaProduction} per monkey x {monkeys.Count()} monkeys)");
        }
        else
        {
            Debug.LogError("BananaFarm: BananaManager.instance is null!");
        }
    }

    public void Upgrade()
    {
        if (level >= MAX_LEVEL)
        {
            Debug.Log($"Banana Farm is already at max level ({MAX_LEVEL})!");
            return;
        }
        
        int upgradeCost = GetUpgradeCost();
        
        if (BananaManager.instance == null)
        {
            Debug.LogError("BananaManager.instance is null!");
            return;
        }
        
        if (BananaManager.instance.GetBananas() < upgradeCost)
        {
            Debug.Log($"Not enough bananas! Need {upgradeCost}, have {BananaManager.instance.GetBananas()}");
            return;
        }
        
        BananaManager.instance.AddBananas(-upgradeCost);
        
        level++;
        
        // building level is used as a multiplier for the banana production
        switch (level)
        {
            case 1:
                buildingLevel = 1;
                break;
            case 2:
                buildingLevel = 2;
                break;
            case 3:
                buildingLevel = 3;
                break;
            case 4:
                buildingLevel = 5;
                break;
        }
        
        if (upgradeEffect != null)
        {
            upgradeEffect.Play();
            Debug.Log("[BananaFarm] Playing upgrade effect!");
        }
        else
        {
            Debug.LogWarning("[BananaFarm] Upgrade effect is NULL!");
        }
        
        Debug.Log($"Banana Farm upgraded to level {level}! Now produces {baseBananaProduction} bananas per monkey per day.");
    }
    
    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    public int GetBananasPerDay() => baseBananaProduction;
    
    public int GetUpgradeCost()
    {
        if (level >= MAX_LEVEL) return 0;
        return upgradeCosts[level + 1];
    }

    public override void OnDestroy()
    {
        if (monkeys != null)
        {
            monkeys.FreeMonkeys();
        }
    }
}