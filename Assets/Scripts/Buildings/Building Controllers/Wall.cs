using UnityEngine;

public class Wall : BuildingBase
{
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.Wall);
        monkeys = new(0);
    }

    public override void OnDayCycle()
    {
        // Walls currently have no special behavior per day
    }

    public override void OnDestroy()
    {
        // Walls currently have no destruction side effects
    }
}

