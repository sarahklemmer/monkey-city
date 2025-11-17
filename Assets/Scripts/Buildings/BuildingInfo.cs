using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BuildingInfo : MonoBehaviour
{
    [SerializeField] GameObject upgradeButton;
    [SerializeField] GameObject moveButton;
    [SerializeField] GameObject info;
    [SerializeField] GameObject backgroundBlocker; 
    
    [Header("Info Display")]
    [SerializeField] TextMeshProUGUI buildingNameText;
    [SerializeField] TextMeshProUGUI buildingStatsText;
    [SerializeField] TextMeshProUGUI upgradeInfoText;
    [SerializeField] BuildingManager manager;
    
    [Header("Positioning")]
    [SerializeField] Vector2 offset = new Vector2(150, 0);
    
    [SerializeField] private string wallUnlockToast = "New defenses available! Open the Building Menu to construct walls.";
    private bool wallToastShown = false;
    
    [Header("Upgrade Settings")]
    [SerializeField] private int archerTowerLevel2Cost = 50;
    [SerializeField] private int archerTowerLevel3Cost = 120;
    
    public static BuildingInfo instance;
    private RectTransform panelRect;
    private Canvas canvas;

    private bool shownOnce = false;
    private bool hiddenOnce = false;
    private BuildingBase current = null;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BuildingInfo on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
        
        if (info != null)
            panelRect = info.GetComponent<RectTransform>();
        
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        Hide();
    }

    void Update() {
        if (current != null) DisplayBuildingInfo(current);
    }

    public void Show(BuildingBase building)
    {
        GlobalInteractionLock.Lock();
        if (building == null) return;
        
        if (info == null)
        {
            Debug.LogError("BuildingInfo UI elements not assigned in Inspector!");
            return;
        }

        current = building;
        shownOnce = true;
        info.SetActive(true);
        
        if (backgroundBlocker != null)
            StartCoroutine(EnableBackgroundBlockerDelayed());
        
        PositionPanelNearBuilding(building);
        //DisplayBuildingInfo(building);

        moveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        if (building is not TreeOfLife)
        {
            moveButton.SetActive(true);
            moveButton.GetComponent<Button>().onClick.AddListener(() =>
            {   
                Hide(true);
                MoveButtonOnClick.ClickHandler(building);
            });
        } else
        {
            moveButton.SetActive(false);
        }

        // Handle upgrade button for Archer Tower
        if (building is ArcherTower && upgradeButton != null)
        {
            ArcherTower archer = building as ArcherTower;
            upgradeButton.SetActive(true);
            
            Button upgradeBtn = upgradeButton.GetComponent<Button>();
            upgradeBtn.onClick.RemoveAllListeners();
            upgradeBtn.onClick.AddListener(() => {
                UpgradeArcherTower(archer);
            });

            if (upgradeInfoText != null)
            {
                upgradeInfoText.gameObject.SetActive(true);
                if (archer.IsMaxLevel())
                {
                    upgradeInfoText.text = "MAX LEVEL";
                    upgradeBtn.interactable = false;
                }
                else if (archer.GetLevel() == 2) // Trying to upgrade to level 3
                {
                    // Check if Tree of Life is level 2
                    TreeOfLife tree = FindFirstObjectByType<TreeOfLife>();
                    if (tree != null && tree.GetLevel() < 2)
                    {
                        upgradeInfoText.text = "Requires Tree of Life Level 2";
                        upgradeBtn.interactable = false;
                    }
                    else
                    {
                        int cost = archer.GetUpgradeCost();
                        upgradeInfoText.text = $"Upgrade Cost: {cost} Bananas\nIncreased range & faster shooting";
                        upgradeBtn.interactable = BananaManager.instance.GetBananas() >= cost;
                    }
                }
                else // Level 1 -> 2
                {
                    int cost = archer.GetUpgradeCost();
                    upgradeInfoText.text = $"Upgrade Cost: {cost} Bananas\nFaster shooting";
                    upgradeBtn.interactable = BananaManager.instance.GetBananas() >= cost;
                }
            }
        }
        // Handle upgrade button for Banana Farm
        else if (building is BananaFarm && upgradeButton != null)
        {
            BananaFarm farm = building as BananaFarm;
            upgradeButton.SetActive(true);
            
            Button upgradeBtn = upgradeButton.GetComponent<Button>();
            upgradeBtn.onClick.RemoveAllListeners();
            upgradeBtn.onClick.AddListener(() => {
                UpgradeBananaFarm(farm);
            });

            if (upgradeInfoText != null)
            {
                upgradeInfoText.gameObject.SetActive(true);
                if (farm.IsMaxLevel())
                {
                    upgradeInfoText.text = "MAX LEVEL";
                    upgradeBtn.interactable = false;
                }
                else if (farm.GetLevel() + 1 > 2) // Trying to upgrade PAST level 2
                {
                    // Check if Tree of Life is level 2
                    TreeOfLife tree = FindFirstObjectByType<TreeOfLife>();
                    if (tree != null && tree.GetLevel() < 2)
                    {
                        upgradeInfoText.text = "Requires Tree of Life Level 2";
                        upgradeBtn.interactable = false;
                    }
                    else
                    {
                        int cost = farm.GetUpgradeCost();
                        upgradeInfoText.text = $"Upgrade Cost: {cost} Bananas\nNext: {farm.GetLevel() + 1}x production";
                        upgradeBtn.interactable = BananaManager.instance.GetBananas() >= cost;
                    }
                }
                else
                {
                    int cost = farm.GetUpgradeCost();
                    upgradeInfoText.text = $"Upgrade Cost: {cost} Bananas\nNext: {farm.GetLevel() + 1}x production";
                    upgradeBtn.interactable = BananaManager.instance.GetBananas() >= cost;
                }
            }
        }
        // Handle upgrade button for Tree of Life
        else if (building is TreeOfLife && upgradeButton != null)
        {
            TreeOfLife tree = building as TreeOfLife;
            upgradeButton.SetActive(true);
            
            Button upgradeBtn = upgradeButton.GetComponent<Button>();
            upgradeBtn.onClick.RemoveAllListeners();
            upgradeBtn.onClick.AddListener(() => {
                UpgradeTreeOfLife(tree);
            });

            if (upgradeInfoText != null)
            {
                upgradeInfoText.gameObject.SetActive(true);
                if (tree.IsMaxLevel())
                {
                    upgradeInfoText.text = "MAX LEVEL";
                    upgradeBtn.interactable = false;
                }
                else
                {
                    upgradeInfoText.text = $"Upgrade Cost: 100 Bananas\nUnlocks higher building levels";
                    upgradeBtn.interactable = BananaManager.instance.GetBananas() >= 100;
                }
            }
        }
        else
        {
            if (upgradeButton != null)
                upgradeButton.SetActive(false);
            if (upgradeInfoText != null)
            {
                upgradeInfoText.gameObject.SetActive(false);
                upgradeInfoText.text = "";
            }
        }
    }

    private void DisplayBuildingInfo(BuildingBase building)
    {
        if (building is ArcherTower)
        {
            ArcherTower archer = building as ArcherTower;
            if (buildingNameText != null)
                buildingNameText.text = "Archer Tower";
            
            if (buildingStatsText != null)
            {
                buildingStatsText.text = 
                    $"Level: {archer.GetLevel()}\n" +
                    $"Range: {archer.GetAttackRange():F1}m\n" +
                    $"Damage: {AllArcherTowerInfo.instance.GetDamagePerAttack():F1}\n" +
                    $"Attack Speed: {archer.GetAttackCooldown():F2}s\n" +
                    $"Monkeys: {building.GetMonkeyCount()}/{building.GetMonkeyCapacity()}";
            }
        }
        else if (building is BananaFarm)
        {
            BananaFarm farm = building as BananaFarm;
            if (buildingNameText != null)
                buildingNameText.text = "Banana Farm";
            
            if (buildingStatsText != null)
            {
                int totalProduction = farm.GetBananasPerDay() * building.GetMonkeyCount() * 2;
                buildingStatsText.text = 
                    $"Level: {farm.GetLevel()}\n" +
                    $"Production: {farm.bananasToProduce * 3} Bananas/day\n" +
                    $"Per Monkey: {(farm.bananasToProduce / building.GetMonkeyCount()) * 3} Bananas/day\n" +
                    $"Monkeys: {building.GetMonkeyCount()}/{building.GetMonkeyCapacity()}";
            }
        }
        else if (building is TreeOfLife)
        {
            TreeOfLife tree = building as TreeOfLife;
            if (buildingNameText != null)
                buildingNameText.text = "Tree of Life";
            
            if (buildingStatsText != null)
            {
                buildingStatsText.text =
                    $"Level: {tree.GetLevel()}\n" +
                    "Your base - Protect at all costs!\n" +
                    $"Idle Monkeys: {building.GetMonkeyCount()}";
            }
        }
        else
        {
            if (buildingNameText != null)
                buildingNameText.text = building.GetType().Name;
            
            if (buildingStatsText != null)
                buildingStatsText.text = $"Monkeys: {building.GetMonkeyCount()}/{building.GetMonkeyCapacity()}";
        }
    }

    private void UpgradeArcherTower(ArcherTower archer)
    {
        if (archer.IsMaxLevel())
        {
            return;
        }

        int cost = archer.GetUpgradeCost();
        if (BananaManager.instance.GetBananas() >= cost)
        {
            BananaManager.instance.AddBananas(-cost);
            manager.ShowFloatingText(archer, -cost);
            archer.Upgrade();
            PlayUpgradeEffect(archer);
            
            if (archer.GetLevel() == 3 && !wallToastShown)
            {
                wallToastShown = true;
                ToastManager.Instance.RequestToast(wallUnlockToast, 4f);
            }
            
            Hide();
        }
    }

    private void UpgradeBananaFarm(BananaFarm farm)
    {
        if (farm.IsMaxLevel())
        {
            Debug.Log("Banana Farm is already max level!");
            return;
        }

        int cost = farm.GetUpgradeCost();
        if (BananaManager.instance.GetBananas() >= cost)
        {
            farm.Upgrade();
            PlayUpgradeEffect(farm);
            Hide();
            Debug.Log($"Banana Farm upgraded to level {farm.GetLevel()}!");
        }
        else
        {
            Debug.Log($"Not enough bananas to upgrade! Need {cost}, have {BananaManager.instance.GetBananas()}");
        }
    }

    private void UpgradeTreeOfLife(TreeOfLife tree)
    {
        if (tree.IsMaxLevel())
        {
            Debug.Log("Tree of Life is already max level!");
            return;
        }

        int cost = 100;
        if (BananaManager.instance.GetBananas() >= cost)
        {
            BananaManager.instance.AddBananas(-cost);
            tree.Upgrade();
            
            PlayUpgradeEffect(tree);
            Hide();
            Debug.Log($"Tree of Life upgraded to level {tree.GetLevel()}! Higher building levels unlocked!");
        }
        else
        {
            Debug.Log($"Not enough bananas to upgrade! Need {cost}, have {BananaManager.instance.GetBananas()}");
        }
    }

    private void PlayUpgradeEffect(BuildingBase building)
    {
        // Look for a particle system in the building's children
        ParticleSystem upgradeEffect = building.GetComponentInChildren<ParticleSystem>();
        
        if (upgradeEffect != null)
        {
            upgradeEffect.Play();
            Debug.Log($"[BuildingInfo] Playing upgrade effect for {building.GetType().Name}");
        }
        else
        {
            Debug.LogWarning($"[BuildingInfo] No upgrade effect found for {building.GetType().Name}");
        }
    }

    private void PositionPanelNearBuilding(BuildingBase building)
    {
        if (panelRect == null || canvas == null) return;

        Vector3 buildingWorldPos = building.transform.position;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(buildingWorldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            canvas.worldCamera,
            out Vector2 canvasPos
        );

        canvasPos += offset;

        float halfWidth = panelRect.rect.width / 2;
        float halfHeight = panelRect.rect.height / 2;
        
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float maxX = canvasRect.rect.width / 2 - halfWidth;
        float maxY = canvasRect.rect.height / 2 - halfHeight;
        
        canvasPos.x = Mathf.Clamp(canvasPos.x, -maxX, maxX);
        canvasPos.y = Mathf.Clamp(canvasPos.y, -maxY, maxY);

        panelRect.anchoredPosition = canvasPos;
    }

    private IEnumerator EnableBackgroundBlockerDelayed()
    {
        yield return null;
        
        if (backgroundBlocker != null)
            backgroundBlocker.SetActive(true);
    }

    public void Hide(bool moving = false)
    {
        if(!moving) GlobalInteractionLock.Unlock();
        current = null;
        Debug.Log("hiding");

        if (shownOnce) hiddenOnce = true;
        upgradeButton.SetActive(false);
        if (upgradeInfoText != null)
            upgradeInfoText.gameObject.SetActive(false);
        info.SetActive(false); 
        backgroundBlocker.SetActive(false);
        upgradeButton.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public bool ShownAndHidden() => hiddenOnce;
}