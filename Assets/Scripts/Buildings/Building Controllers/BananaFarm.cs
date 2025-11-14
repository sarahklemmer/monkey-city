public class BananaFarm : BuildingBase
{
    //TODO: make this scale with upgrades
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.BananaFarm); 
        monkeys = new(2);
    }

    void Update()
    {
        //TODO: move this out of update this is really shitty and not performant 
        bananasPerDay = monkeys.Count() * 2;
    }

    public override void OnDayCycle()
    {
        // do nothing
    }

    public override void OnDestroy()
    {
        monkeys.FreeMonkeys();
    }
}
