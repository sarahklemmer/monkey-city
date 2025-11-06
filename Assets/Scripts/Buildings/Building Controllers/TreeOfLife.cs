public class NewMonoBehaviourScript : BuildingBase
{
    void Awake()
    {
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
