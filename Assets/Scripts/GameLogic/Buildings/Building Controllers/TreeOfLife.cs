using UnityEngine;
using UnityEngine.Assertions;

public class TreeOfLife : BuildingBase
{
    private int level = 1;
    private const int MAX_LEVEL = 2;
    
    [SerializeField] ParticleSystem upgradeEffect; 
    
    public static event System.Action OnTreePlaced;
    
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
        
        // Play building placement sound
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
        if(level == MAX_LEVEL) canNeverBeUpgraded = true;
    }

    public override string GetUpgradeText() => 
        level == MAX_LEVEL ? 
        "Max Level" : 
        "Upgrade Cost: 100 Bananas\nUnlocks higher building levels";

    // D:
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

    public override void OnDestroy()
    {
        TimeController.instance.StopTicking();
        BananaProductionTimer.instance.StopProduction();
        WaveSpawner.instance.PauseSpawning();
        DeathScreen.instance.ShowDeathScreen();
    }
}