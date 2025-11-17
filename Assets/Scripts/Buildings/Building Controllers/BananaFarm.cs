using UnityEngine;

public class BananaFarm : BuildingBase
{
    private int level = 1;
    private const int MAX_LEVEL = 4;
    private int bananasPerDay = 1;
    private static readonly int[] upgradeCosts = { 0, 5, 40, 100, 200 };
    
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.BananaFarm); 
        monkeys = new(2);
    }

    public override void OnDayCycle()
    {
        if (monkeys != null && monkeys.Count() > 0)
        {
            BananaManager.instance.AddBananas(bananasPerDay * monkeys.Count());
        }
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
                bananasPerDay = 1;
                break;
            case 2:
                bananasPerDay = 2;
                break;
            case 3:
                bananasPerDay = 3;
                break;
            case 4:
                bananasPerDay = 5;
                break;
        }
        
        UnityEngine.Debug.Log($"Banana Farm upgraded to level {level}! Now produces {bananasPerDay} bananas per monkey per day.");
    }
    
    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    public int GetBananasPerDay() => bananasPerDay;
    
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
