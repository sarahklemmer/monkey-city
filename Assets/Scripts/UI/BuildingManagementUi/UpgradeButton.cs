using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [HideInInspector] public BuildingBase building;
    [SerializeField] Button button;
    bool upgradeToastSpawned = false;

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
        } else
        {
            //TODO: update this when we add upgrades for traps (if we add upgrades for traps)
            button.interactable = false;
        }

        if(button.interactable && !upgradeToastSpawned && BuildingManager.instance.GetTreeOfLife() != null) {
            upgradeToastSpawned = true;
            ToastManager.Instance.RequestToast("You've earned enough bananas to upgrade a building! Click a green arrow on the right to upgrade");
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
