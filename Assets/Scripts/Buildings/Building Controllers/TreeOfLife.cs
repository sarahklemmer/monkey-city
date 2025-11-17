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
        building = new(BuildingType.TreeOfLife); 
        monkeys = new(999);
    }
    
    void Start()
    {
        NotifyTreePlaced();
        SimpleTutorial.instance.StartTutorial();
        BuildingUnlock.Unlock(BuildingType.BananaFarm);
        BuildingUnlock.Unlock(BuildingType.ArcherTower);
        BuildingUnlock.Disable(BuildingType.TreeOfLife);
    }
    
    public void NotifyTreePlaced()
    {
        OnTreePlaced?.Invoke();
    }
    
    public void Upgrade()
    {
        if (level >= MAX_LEVEL)
        {
            UnityEngine.Debug.Log($"Tree of Life is already at max level ({MAX_LEVEL})!");
            return;
        }
        
        level++;
        
        if (upgradeEffect != null)
        {
            upgradeEffect.Play();
            Debug.Log("[TreeOfLife] Playing upgrade effect!");
        }
        else
        {
            Debug.LogWarning("[TreeOfLife] Upgrade effect is NULL!");
        }
        
        UnityEngine.Debug.Log($"Tree of Life upgraded to level {level}!");
        
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
        //TODO: make restarting MUCH more polished
        SceneLoader.instance.ReloadScene();
    }
}