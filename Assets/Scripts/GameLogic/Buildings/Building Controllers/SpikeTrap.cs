using UnityEngine;

public class SpikeTrap : BuildingBase
{
    [SerializeField] float baseDamageMultiplier = 2f;
    public float damageMultiplier { get; private set; }
    
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.SpikeTrap);
        monkeys = new(1);

        canContainMonkeys = false;
    }

    void Start()
    {
        damageMultiplier = baseDamageMultiplier;
    }

    public override void OnDayCycle()
    {
        return;
    }

    public override void OnDestroy()
    {
        return;
    }
}
