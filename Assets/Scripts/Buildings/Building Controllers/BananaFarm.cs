public class BananaFarm : BuildingBase
{
    void Awake()
    {
        building = new(BuildingType.BananaFarm); 
    }

    public override void OnDayCycle()
    {
        BananaManager.instance.AddBananas(1);
    }

    public override void OnDestroy()
    {
        /* do nothing */
    }
}
