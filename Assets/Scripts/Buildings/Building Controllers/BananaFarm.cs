public class BananaFarm : BuildingBase
{
    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.BananaFarm); 
        monkeys = new(2);
    }

    public override void OnDayCycle()
    {
        // add a banana for each monkey we have allocated
        BananaManager.instance.AddBananas(AllBananaFarmInfo.instance.GetBananasPerDay() * monkeys.Count());
    }

    public override void OnDestroy()
    {
        monkeys.FreeMonkeys();
    }
}
