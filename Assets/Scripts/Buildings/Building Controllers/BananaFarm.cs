using UnityEngine;

public class BananaFarm : BuildingBase
{
    private int level = 1;
    private const int MAX_LEVEL = 4;
    private int baseBananaProduction = 1;
    private static readonly int[] upgradeCosts = { 0, 5, 40, 100, 200 };
    
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.BananaFarm); 
        monkeys = new(2);
    }

    void Update()
    {
        bananasPerDay = baseBananaProduction * monkeys.Count() * 2;
    }

    public override void OnDayCycle()
    {

    }

    public void Upgrade()
    {
        if (level >= MAX_LEVEL)
        {
            UnityEngine.Debug.Log($"Banana Farm is already at max level ({MAX_LEVEL})!");
            return;
        }
        
        int upgradeCost = GetUpgradeCost();
        
        if (BananaManager.instance.GetBananas() < upgradeCost)
        {
            UnityEngine.Debug.Log($"Not enough bananas! Need {upgradeCost}, have {BananaManager.instance.GetBananas()}");
            return;
        }
        
        BananaManager.instance.AddBananas(-upgradeCost);
        level++;
        
        switch (level)
        {
            case 1:
                baseBananaProduction = 1;
                break;
            case 2:
                baseBananaProduction = 2;
                break;
            case 3:
                baseBananaProduction = 3;
                break;
            case 4:
                baseBananaProduction = 5;
                break;
        }
        
        UnityEngine.Debug.Log($"Banana Farm upgraded to level {level}! Now produces {baseBananaProduction} bananas per monkey per day.");
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
