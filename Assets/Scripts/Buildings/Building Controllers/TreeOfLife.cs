public class TreeOfLife : BuildingBase
{
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.TreeOfLife); 
    }

    public override void OnDayCycle()
    {
        /* do nothing */
    }

    public override void OnDestroy()
    {
        //TODO: LOSE
    }
}
