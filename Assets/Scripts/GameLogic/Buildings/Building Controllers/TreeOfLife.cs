using UnityEngine;
using UnityEngine.Assertions;
public class TreeOfLife : BuildingBase
{
    private int level = 1;
    private const int MAX_LEVEL = 2;
    
    [SerializeField] ParticleSystem upgradeEffect; 
    
    public static event System.Action OnTreePlaced;
    
    private static bool hasCompletedTutorial = false;
    
    void Awake()
    {
        base.SharedAwakeBehavior();
        type = BuildingType.TreeOfLife;
        canNeverBeUpgraded = false;
        monkeys = new(999);
    }
    
    void Start()
    {
        NotifyTreePlaced();
        
        if (BuildingSoundManager.instance != null)
        {
            BuildingSoundManager.instance.PlayBuildingPlacedSound();
        }
        PopulationManager.instance.AddToPopulation(1);
        Soundtrack.instance.PlaySoundtrack();
    }
    
    public void NotifyTreePlaced()
    {
        OnTreePlaced?.Invoke();
    }
    
    public static bool ShouldPlayTutorial()
    {
        return !hasCompletedTutorial;
    }
    
    public static void MarkTutorialComplete()
    {
        hasCompletedTutorial = true;
        Debug.Log("[TreeOfLife] Tutorial marked as complete - will not play again");
    }
    
    public static void ResetTutorialFlag()
    {
        hasCompletedTutorial = false;
        Debug.Log("[TreeOfLife] Tutorial flag reset");
    }
    
    public override bool CanUpgrade() => GetUpgradeCost() <= BananaManager.instance.GetBananas() && !canNeverBeUpgraded;
    
    public override bool AttemptUpgrade()
    {
        if(!CanUpgrade()) return false;
        BananaManager.instance.AddBananas(-GetUpgradeCost());
        Upgrade();
        return true;
    }
    
    public override int GetUpgradeCost()
    {
        if (level >= MAX_LEVEL) return 0;
        return 100;
    }
    
    public void Upgrade()
    {
        Assert.IsFalse(level >= MAX_LEVEL, "upgrading when we're already at or abvoe? max level!");
        level++;
        
        BuildingSoundManager.instance.PlayUpgradeSound();
        upgradeEffect.Play();
        TreeOfLifeUpgradeEffects modelUpgradeEffect = GetComponent<TreeOfLifeUpgradeEffects>();
        Assert.IsNotNull(modelUpgradeEffect, "TreeOfLifeUpgradeEffects not attached to prefab");
        modelUpgradeEffect.Upgrade();
        if(level == MAX_LEVEL) canNeverBeUpgraded = true;
    }
    
    public override string GetUpgradeText() => 
        level == MAX_LEVEL ? 
        "Max Level" : 
        "Upgrade Cost: 100 Bananas\nUnlocks higher building levels";
    
    public override string GetDescription() => 
                    $"Level: {GetLevel()}\n" +
                    "Your base - Protect at all costs!\n" +
                    $"Idle Monkeys: {GetMonkeyCount()}";
    
    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    
    public override void OnDayCycle()
    {   
        /* do nothing */
    }
    
    public override void Die()
    {
        Debug.Log("[TreeOfLife] Die() called - Triggering game over");
        
        if (TimeController.instance != null)
        {
            Debug.Log("[TreeOfLife] Stopping TimeController");
            TimeController.instance.StopTicking();
        }
        
        if (BananaProductionTimer.instance != null)
        {
            Debug.Log("[TreeOfLife] Stopping BananaProductionTimer");
            BananaProductionTimer.instance.StopProduction();
        }
        
        if (WaveSpawner.instance != null)
        {
            Debug.Log("[TreeOfLife] Pausing WaveSpawner");
            WaveSpawner.instance.PauseSpawning();
        }
        
        if (DeathScreen.instance != null)
        {
            Debug.Log("[TreeOfLife] Showing DeathScreen");
            DeathScreen.instance.ShowDeathScreen();
        }
        else
        {
            Debug.LogError("[TreeOfLife] DeathScreen.instance is NULL!");
        }
        
        base.Die();
    }
    
    public override void OnDestroy()
    {
        Debug.Log("[TreeOfLife] OnDestroy called");
    }
}