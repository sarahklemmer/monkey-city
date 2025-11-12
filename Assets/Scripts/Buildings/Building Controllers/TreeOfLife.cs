public class TreeOfLife : BuildingBase
{
    // Add this static event for hiding the UI button
    public static event System.Action OnTreePlaced;
    
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.TreeOfLife); 
        monkeys = new(0);
    }
    
    void Start()
    {
        // Notify that the tree has been placed
        NotifyTreePlaced();
    }
    
    public void NotifyTreePlaced()
    {
        OnTreePlaced?.Invoke();
    }

    public override void OnDayCycle()
    {   
        // TODO: replace this with real, good monkey spawning logic
        // add a monkey every 2 days
        if(TimeController.instance.currentDay % 2 == 0 && TimeController.instance.currentDay > 0)
        {
            PopulationManager.instance.AddToPopulation(1);
        }
        /* do nothing */
    }

    public override void OnDestroy()
    {
        //TODO: make restarting MUCH more polished
        SceneLoader.instance.ReloadScene();
    }
}