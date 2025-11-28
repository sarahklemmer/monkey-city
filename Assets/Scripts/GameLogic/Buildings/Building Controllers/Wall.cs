public class Wall : BuildingBase
{
    void Awake()
    {
        base.SharedAwakeBehavior();
        type = BuildingType.Wall;
        monkeys = new(0);
        canContainMonkeys = false;
    }

    public override void OnDayCycle()
    {
        // Walls currently have no special behavior per day
    }

    public override void OnDestroy()
    {
        // Walls currently have no destruction side effects
    }

    public override string GetDescription() => "wall";
}

