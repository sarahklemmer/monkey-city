using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [HideInInspector] public BuildingBase building;
    [SerializeField] Button button;

    void Start()
    {
        Assert.IsNotNull(button, "forgot to assign add button in the inspector!");
        button.onClick.AddListener(() => Click());
    }

    void Update()
    {
        if(building == null)
        {
            button.interactable = false;
            return;
        }

        if(building.GetBuildingType() == BuildingType.BananaFarm)
        {
            BananaFarm farm = building as BananaFarm;
            button.interactable = !farm.IsMaxLevel() && BananaManager.instance.GetBananas() >= farm.GetUpgradeCost();
        } else if(building.GetBuildingType() == BuildingType.ArcherTower)
        {
            ArcherTower archer = building as ArcherTower;
            button.interactable = !archer.IsMaxLevel() && BananaManager.instance.GetBananas() >= archer.GetUpgradeCost();
        }
    }
    
    public void Click()
    {
        if(building.GetBuildingType() == BuildingType.BananaFarm)
            UpgradeBananaFarm(building as BananaFarm);
        else if(building.GetBuildingType() == BuildingType.ArcherTower)
            UpgradeArcherTower(building as ArcherTower);

        BuildingInfo.instance.PlayUpgradeEffect(building);
    }

    private void UpgradeArcherTower(ArcherTower archer)
    {
        if (archer.IsMaxLevel())
        {
            return;
        }
        //TODO: wall stuff
        int cost = archer.GetUpgradeCost();
        BananaManager.instance.AddBananas(-cost);
        archer.Upgrade();
    }

    private void UpgradeBananaFarm(BananaFarm farm)
    {
        farm.Upgrade();
    }
}
