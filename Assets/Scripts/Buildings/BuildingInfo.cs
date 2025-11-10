using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BuildingInfo : MonoBehaviour
{
    [SerializeField] GameObject removeButton;
    [SerializeField] GameObject upgradeButton;
    [SerializeField] GameObject info;
    [SerializeField] GameObject backgroundBlocker; 
    
    [Header("Info Display")]
    [SerializeField] TextMeshProUGUI buildingNameText;
    [SerializeField] TextMeshProUGUI buildingStatsText;
    [SerializeField] TextMeshProUGUI upgradeInfoText;
    
    [Header("Positioning")]
    [SerializeField] Vector2 offset = new Vector2(150, 0);
    
    public static BuildingInfo instance;
    private BuildingBase currentBuilding;
    private RectTransform panelRect;
    private Canvas canvas;

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

    public void Show(BuildingBase building)
    {
        if (building == null) return;
        
        if (info == null || removeButton == null)
        {
            Debug.LogError("BuildingInfo UI elements not assigned in Inspector!");
            return;
        }

        currentBuilding = building;
        info.SetActive(true);
        
        // Check if this is a TreeOfLife - if so, hide remove button
        if (building is TreeOfLife)
        {
            removeButton.SetActive(false);
        }
        else
        {
            removeButton.SetActive(true);
            
            // Setup remove button to destroy the building
            Button removeBtn = removeButton.GetComponent<Button>();
            removeBtn.onClick.RemoveAllListeners();
            removeBtn.onClick.AddListener(() => {
                RemoveBuilding(building);
            });
        }
        
        if (backgroundBlocker != null)
            StartCoroutine(EnableBackgroundBlockerDelayed());
        
        PositionPanelNearBuilding(building);
        DisplayBuildingInfo(building);

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
                else
                {
                    upgradeInfoText.text = $"Upgrade Cost: 300 Bananas";
                    upgradeBtn.interactable = BananaManager.instance.GetBananas() >= 300;
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

    private void RemoveBuilding(BuildingBase building)
    {
        if (building == null) return;
        
        // Deallocate all monkeys from this building
        while (building.GetMonkeyCount() > 0)
        {
            MonkeyController monkey = building.NextMonkeyToRemove();
            if (monkey != null)
            {
                monkey.allocation.Deallocate();
            }
        }
        
        // Clear the grid space
        if (BuildingGrid.instance != null)
        {
            BuildingGrid.instance.ClearBuildingFromGrid(building);
        }
        
        // Hide the UI
        Hide();
        
        // Destroy the building GameObject
        Destroy(building.gameObject);
        
        Debug.Log($"Building {building.GetType().Name} removed!");
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
                    $"Damage: {archer.GetAttackDamage():F1}\n" +
                    $"Attack Speed: {archer.GetAttackCooldown():F2}s\n" +
                    $"Monkeys: {building.GetMonkeyCount()}/{building.GetMonkeyCapacity()}";
            }
        }
        else if (building is BananaFarm)
        {
            if (buildingNameText != null)
                buildingNameText.text = "Banana Farm";
            
            if (buildingStatsText != null)
            {
                buildingStatsText.text = 
                    $"Production: 1 Banana per day\n" +
                    $"Monkeys: {building.GetMonkeyCount()}/{building.GetMonkeyCapacity()}";
            }
        }
        else if (building is TreeOfLife)
        {
            if (buildingNameText != null)
                buildingNameText.text = "Tree of Life";
            
            if (buildingStatsText != null)
            {
                buildingStatsText.text = "Your base - Protect at all costs!";
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
            Debug.Log("Archer Tower is already max level!");
            return;
        }

        if (BananaManager.instance.GetBananas() >= 300)
        {
            BananaManager.instance.RemoveBananas(300);
            archer.Upgrade();
            Show(archer);
            Debug.Log("Archer Tower upgraded!");
        }
        else
        {
            Debug.Log("Not enough bananas to upgrade!");
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

    private System.Collections.IEnumerator EnableBackgroundBlockerDelayed()
    {
        yield return null;
        
        if (backgroundBlocker != null)
            backgroundBlocker.SetActive(true);
    }

    public void Hide()
    {
        Debug.Log("hiding");
        currentBuilding = null;

        removeButton.SetActive(false);
        upgradeButton.SetActive(false);
        upgradeInfoText.gameObject.SetActive(false);
        info.SetActive(false);
        backgroundBlocker.SetActive(false);
        removeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        upgradeButton.GetComponent<Button>().onClick.RemoveAllListeners();
        if (Tutorial.instance.tutorialActive && Tutorial.instance.tutorialStage == 6) Tutorial.instance.PlayerClosesBananaFarmWindow();
    }
}