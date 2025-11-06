public class ArcherTower : BuildingBase
{
    void Awake()
    {
        building = new(BuildingType.TreeOfLife);
    }

    void Update()
    {
        //TODO: attack monkeys
    }

    public override void OnDayCycle()
    {
        /* do nothing */
    }

    public override void OnDestroy()
    {
        /* do nothing */
    }
}
