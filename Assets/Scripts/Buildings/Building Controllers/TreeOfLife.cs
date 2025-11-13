public class TreeOfLife : BuildingBase
{
    // Add this static event for hiding the UI button
    public static event System.Action OnTreePlaced;
    
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.TreeOfLife); 
        monkeys = new(999);
    }
    
    void Start()
    {
        // Notify that the tree has been placed
        NotifyTreePlaced();
        SimpleTutorial.instance.StartTutorial();
        // unlock other buildings
        BuildingUnlock.Unlock(BuildingType.BananaFarm);
        BuildingUnlock.Unlock(BuildingType.ArcherTower);
        BuildingUnlock.Disable(BuildingType.TreeOfLife);
    }
    
    public void NotifyTreePlaced()
    {
        OnTreePlaced?.Invoke();
    }

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