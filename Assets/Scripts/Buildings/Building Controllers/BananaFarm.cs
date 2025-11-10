public class BananaFarm : BuildingBase
{
    //TODO: make this scale with upgrades
    private int bananasPerDay = 1;
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.BananaFarm); 
        monkeys = new(2);
    }

    public override void OnDayCycle()
    {
        // add a banana for each monkey we have allocated
        BananaManager.instance.AddBananas(bananasPerDay * monkeys.Count() * 2);
    }

    public override void OnDestroy()
    {
        monkeys.FreeMonkeys();
    }
}
