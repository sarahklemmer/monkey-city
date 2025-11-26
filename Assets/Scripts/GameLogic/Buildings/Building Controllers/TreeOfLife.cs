using UnityEngine;

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
        
        //TODO: if we want a tutorial uncomment this
        // SimpleTutorial.instance.StartTutorial();
        BuildingUnlock.Unlock(BuildingType.BananaFarm);
        BuildingUnlock.Unlock(BuildingType.ArcherTower);
        BuildingUnlock.Unlock(BuildingType.Beacon);
        BuildingUnlock.Disable(BuildingType.TreeOfLife);
        Soundtrack.instance.PlaySoundtrack();
    }
    
    public void NotifyTreePlaced()
    {
        OnTreePlaced?.Invoke();
    }
    
    public void Upgrade()
    {
        if (level >= MAX_LEVEL)
        {
            Debug.Log($"Tree of Life is already at max level ({MAX_LEVEL})!");
            return;
        }
        
        level++;
        
        // Play upgrade sound
        if (BuildingSoundManager.instance != null)
        {
            BuildingSoundManager.instance.PlayUpgradeSound();
        }
        
        if (upgradeEffect != null)
        {
            upgradeEffect.Play();
            Debug.Log("[TreeOfLife] Playing upgrade effect!");
        }
        else
        {
            Debug.LogWarning("[TreeOfLife] Upgrade effect is NULL!");
        }
        
        Debug.Log($"Tree of Life upgraded to level {level}!");
        
        // You can add visual changes or stat improvements here
        // For example:
        // - Increase monkey capacity
        // - Visual model changes
        // - Special abilities unlock
    }
    
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