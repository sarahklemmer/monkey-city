public class TreeOfLife : BuildingBase
{
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.TreeOfLife); 
        monkeys = new(0);
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
