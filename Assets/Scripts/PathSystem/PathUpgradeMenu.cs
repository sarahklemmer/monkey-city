using UnityEngine;
using TMPro;

public class PathUpgradeMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text currentPathTypeText;

    [Header("Farmer Path")]
    [SerializeField] private TMP_Text farmerLevelText;
    [SerializeField] private TMP_Text farmerProductionText;
    [SerializeField] private TMP_Text farmerBoostText;
    [SerializeField] private TMP_Text farmerUpgradeCostText;

    [Header("Warrior Path")]
    [SerializeField] private TMP_Text warriorLevelText;
    [SerializeField] private TMP_Text warriorDamageText;
    [SerializeField] private TMP_Text warriorBoostText;
    [SerializeField] private TMP_Text warriorUpgradeCostText;

    [Header("Scholar Path")]
    [SerializeField] private TMP_Text scholarLevelText;
    [SerializeField] private TMP_Text scholarMonkeySpeedText;
    [SerializeField] private TMP_Text scholarBoostText;
    [SerializeField] private TMP_Text scholarUpgradeCostText;

    void OnEnable()
    {
        TimeController.instance.StopTicking();
        BananaProductionTimer.instance.StopProduction();
        WaveSpawner.instance.PauseSpawning();
        Refresh();
    }

    void OnDisable()
    {
        TimeController.instance.StartTicking();
        BananaProductionTimer.instance.StartProduction();
        WaveSpawner.instance.UnpauseSpawning();
        // Continues the game from where it was paused
    }

    public void Refresh()
    {
        if (PathManager.instance == null) return;

        currentPathTypeText.text = $"Your Path Type: {PathManager.instance.currentPathType}";

        UpdateFarmerSection();
        UpdateWarriorSection();
        UpdateScholarSection();
    }

    void UpdateFarmerSection()
    {
        var level = PathManager.instance.GetPathLevel(PathType.Farmer);
        farmerLevelText.text = $"Current Farmer Level: {level}";

        float productionRate = AllBananaFarmInfo.instance.GetBananasPerDay();
        farmerProductionText.text = $"Production Rate: {productionRate} bananas/monkey";

        farmerBoostText.text = $"Upgrade Boost: +1 bananas/monkey";
        farmerUpgradeCostText.text = $"Cost of Upgrade: {PathManager.instance.CalculateCostOfUpgrade(PathType.Farmer)} pts";
    }

    void UpdateWarriorSection()
    {
        var level = PathManager.instance.GetPathLevel(PathType.Warrior);
        warriorLevelText.text = $"Current Warrior Level: {level}";

        float damage = AllArcherTowerInfo.instance.GetDamagePerAttack();
        warriorDamageText.text = $"Bullet Damage: {damage}";

        warriorBoostText.text = $"Upgrade Boost: +5 damage";
        warriorUpgradeCostText.text = $"Cost of Upgrade: {PathManager.instance.CalculateCostOfUpgrade(PathType.Warrior)} pts";
    }

    void UpdateScholarSection()
    {
        var level = PathManager.instance.GetPathLevel(PathType.Scholar);
        scholarLevelText.text = $"Current Scholar Level: {level}";

        float monkeySpeed = AllMonkeyInfo.instance.GetMonkeySpeed();
        scholarMonkeySpeedText.text = $"Monkey Speed: {monkeySpeed} m/s";

        scholarBoostText.text = $"Upgrade Boost: +2 m/s";
        scholarUpgradeCostText.text = $"Cost of Upgrade: {PathManager.instance.CalculateCostOfUpgrade(PathType.Scholar)} pts";
    }

    public void UpgradeFarmerPath()
    {
        PathManager.instance.FarmerPathUpgrade();
        Refresh();
    }

    public void UpgradeWarriorPath()
    {
        PathManager.instance.WarriorPathUpgrade();
        Refresh();
    }

    public void UpgradeScholarPath()
    {
        PathManager.instance.ScholarPathUpgrade();
        Refresh();
    }

    public void ClosePathUpgradeMenu()
    {
        gameObject.SetActive(false);
    }
}

