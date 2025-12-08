using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class BananaFarm : BuildingBase
{
    private int level = 1;
    private const int MAX_LEVEL = 4;
    private int baseBananaProduction = 1;
    public int bananasToProduce = 0;
    public int buildingLevel = 1;
    private static readonly int[] upgradeCosts = { 0, 5, 40, 100, 200 };
    bool nextUpgradeIndicatorSpawned = false;

    BananaGrow growEffect;
    
    [SerializeField] ParticleSystem upgradeEffect;
    
    void Awake()
    {
        base.SharedAwakeBehavior();
        canNeverBeUpgraded = false;
        type = BuildingType.BananaFarm;
        monkeys = new(2);
        growEffect = GetComponent<BananaGrow>();
        Assert.IsNotNull(growEffect, "no grow effect attached");
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        if (BuildingSoundManager.instance != null)
        {
            BuildingSoundManager.instance.PlayBuildingPlacedSound();
        }
    }

    void Update()
    {
        base.UpdateBehavior();
        
        if (AllBananaFarmInfo.instance != null)
        {
            int baseProduction = AllBananaFarmInfo.instance.GetBananasPerDay() * monkeys.Count() * buildingLevel;
            float productionBonus = GetTotalProductionBonus();
            bananasToProduce = Mathf.RoundToInt(baseProduction * (1f + productionBonus));
        }
        else
        {
            bananasToProduce = 0;
        }
    }

    public override void OnDayCycle()
    {
    }

    public void ProduceBananas()
    {
        if (BananaManager.instance != null)
        {
            BananaManager.instance.AddBananas(bananasToProduce);
        }
        BananaVisualization.SpawnAtPosition(transform, bananasToProduce);
    }

    public override bool CanUpgrade() 
    {
        TreeOfLife tree = BuildingManager.instance.GetTreeOfLife() as TreeOfLife;
        if (tree == null) return false;
        
        if (canNeverBeUpgraded) return false;
        if (BananaManager.instance == null || BananaManager.instance.GetBananas() < GetUpgradeCost()) return false;
        
        if (level >= 2 && tree.GetLevel() < 2) return false;
        
        return true;
    }

    public override bool AttemptUpgrade()
    {
        if(!CanUpgrade()) return false;
        if (BananaManager.instance != null)
        {
            BananaManager.instance.AddBananas(-GetUpgradeCost());
        }
        Upgrade();
        return true;
    }
    
    public override int GetUpgradeCost()
    {
        if (level >= MAX_LEVEL) return 0;
        return upgradeCosts[level + 1];
    }

    void Upgrade()
    {
        Assert.IsFalse(level >= MAX_LEVEL, "upgrading when we're already at or above? max level!");
        level++;
        BuildingSoundManager.instance.PlayUpgradeSound();
        growEffect.Grow();
        
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
        
        if(level >= MAX_LEVEL) {
            canNeverBeUpgraded = true;
            growEffect.GrowTreetop();
        } else
        {
            nextUpgradeIndicatorSpawned = false;
        }
        upgradeEffect.Play();
    }

    public override string GetUpgradeText()
    {
        if(level < MAX_LEVEL) return 
            level <= 1 || (BuildingManager.instance.GetTreeOfLife() as TreeOfLife).GetLevel() >= 2 ? 
            $"Upgrade Cost: {GetUpgradeCost()} Bananas\nNext Upgrade: {GetLevel() + 1}x production" : 
            "Requires Tree of Life Level 2";
        else return "MAX LEVEL";
    }
    
    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    public int GetBananasPerDay() => baseBananaProduction;

    public override string GetDescription()
    {
        // Calculate base production without buffs
        int baseProduction = AllBananaFarmInfo.instance.GetBananasPerDay() * monkeys.Count() * buildingLevel * 3;
        
        // Get total bonus percentage
        float bonusPercent = GetTotalProductionBonus() * 100f;
        
        // Show buffed production if there are active beacons
        string productionText = $"Production: {bananasToProduce * 3} Bananas/day";
        if (bonusPercent > 0)
        {
            productionText += $" <color=green>(+{bonusPercent:F0}%)</color>";
        }
        
        return $"Level: {GetLevel()}\n" +
               productionText + "\n" +
               $"Per Monkey: {(GetMonkeyCount() == 0 ? 0 : (bananasToProduce / GetMonkeyCount()) * 3)} Bananas/day\n" +
               $"Monkeys: {GetMonkeyCount()}/{GetMonkeyCapacity()}";
    }

    public override void OnDestroy()
    {
        if (monkeys != null)
        {
            monkeys.FreeMonkeys();
        }
    }

    private HashSet<Beacon> beaconBuffs = new HashSet<Beacon>();

    public void AddBeaconBuff(Beacon beacon)
    {
        beaconBuffs.Add(beacon);
    }

    public void RemoveBeaconBuff(Beacon beacon)
    {
        beaconBuffs.Remove(beacon);
    }

    public float GetTotalProductionBonus()
    {
        float bonus = 0f;
        foreach (var beacon in beaconBuffs)
        {
            if (beacon != null && beacon.GetMonkeyCount() > 0)
            {
                bonus += beacon.GetProductionBonus();
            }
        }
        return bonus;
    }
}