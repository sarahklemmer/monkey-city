using UnityEngine;

public class SpikeTrap : BuildingBase
{
    [SerializeField] float baseDamageMultiplier = 2f;
    public float damageMultiplier { get; private set; }
    
    void Awake()
    {
        base.SharedAwakeBehavior();
        type = BuildingType.SpikeTrap;
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

    public override string GetDescription() => 
        "A spike trap to distract attacking chimps, it can buy your archer towers time and will hurt chimps as they attack it.";
}
