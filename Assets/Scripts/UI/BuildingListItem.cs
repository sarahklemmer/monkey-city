using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingListItem : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI count;
    
    [Header("Visual Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;
    
    private BuildingData buildingData;
    private Button button;
    
    void Awake()
    {
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }
    
    void OnEnable()
    {
        // Check visibility whenever this UI element is enabled
        if (!string.IsNullOrEmpty(buildingData.buildingName))
        {
            CheckIfShouldHide();
        }
    }
    
    void OnDisable()
    {
        // Unsubscribe from event
        if (!string.IsNullOrEmpty(buildingData.buildingName) && buildingData.buildingName == "Tree Of Life")
        {
            TreeOfLife.OnTreePlaced -= HandleTreePlaced;
        }
    }

    void Update()
    {
        //TODO: make this sexier :D
        // set alpha depending on whether we can purchase
        Image[] imgs = GetComponentsInChildren<Image>();
        foreach(Image img in imgs)
        {                
            Color c = img.color;
            c.a = BuildingTypeToPrice.GetPrice(buildingData.type) <= BananaManager.instance.GetBananas() ? 1f : 0.2f;
            img.color = c;
        }
    }

    public void Setup(BuildingData data)
    {
        buildingData = data;
        
        Debug.Log($"BuildingListItem Setup called for: {data.buildingName}");
        
        if (iconImage != null)
            iconImage.sprite = data.icon;
            
        if (nameText != null)
            nameText.text = data.buildingName;
            
        if (costText != null)
            costText.text = data.cost.ToString() + " Banana" + (data.cost != 1 ? "s" : "");

        // Only show count for buildings that aren't Tree of Life
        if (count != null)
        {
            // Case-insensitive comparison to handle "Tree of Life" or "Tree Of Life"
            if (data.buildingName.Equals("Tree of Life", System.StringComparison.OrdinalIgnoreCase) ||
                data.buildingName.Equals("Tree Of Life", System.StringComparison.OrdinalIgnoreCase))
            {
                // Hide the count text for Tree of Life
                count.gameObject.SetActive(false);
                Debug.Log("[BuildingListItem] Hiding count for Tree of Life");
            }
            else
            {
                // Show count for other buildings
                int buildingCount = BuildingManager.instance.GetBuildingsOfType(data.type).Count;
                count.text = buildingCount.ToString() + "/5 Built";
                
                if(buildingCount == 5) 
                    count.color = Color.red;
                else 
                    count.color = Color.black;
                    
                count.gameObject.SetActive(true);
                Debug.Log($"[BuildingListItem] Showing count for {data.buildingName}: {count.text}");
            }
        }
        
        // Check immediately after setup if this should be hidden
        CheckIfShouldHide();
        
        // Subscribe to event after buildingData is set
        if (!string.IsNullOrEmpty(buildingData.buildingName) && 
            (buildingData.buildingName.Equals("Tree of Life", System.StringComparison.OrdinalIgnoreCase) ||
             buildingData.buildingName.Equals("Tree Of Life", System.StringComparison.OrdinalIgnoreCase)))
        {
            Debug.Log("Subscribing to TreeOfLife.OnTreePlaced event");
            TreeOfLife.OnTreePlaced += HandleTreePlaced;
        }
    }
    
    private void CheckIfShouldHide()
    {
        if (!string.IsNullOrEmpty(buildingData.buildingName) && buildingData.buildingName == "Tree Of Life")
        {
            TreeOfLife existingTree = FindFirstObjectByType<TreeOfLife>();
            
            Debug.Log($"Checking Tree of Life visibility. Found existing tree: {existingTree != null}");
            
            if (existingTree != null)
            {
                Debug.Log("Hiding Tree of Life button!");
                gameObject.SetActive(false);
            }
        }
    }
    
    private void HandleTreePlaced()
    {
        // Hide this item when Tree of Life is placed
        if (!string.IsNullOrEmpty(buildingData.buildingName) && buildingData.buildingName == "Tree Of Life")
        {
            gameObject.SetActive(false);
        }
    }
    
    public void RefreshVisibility()
    {
        CheckIfShouldHide();
    }
    
    // This runs when the button is clicked
    public void OnClick()
    {
        Debug.Log("Selected building: " + buildingData.buildingName);
        // if (iconImage != null)
        //     iconImage.color = selectedColor;
        // TODO: Add logic to handle building selection/purchase
    }
    
    public void OnHoverEnter()
    {
        if (iconImage != null)
            iconImage.color = hoverColor;
    }
    
    public void OnHoverExit()
    {
        if (iconImage != null)
            iconImage.color = normalColor;
    }
    
    public BuildingData GetBuildingData()
    {
        return buildingData;
    }
}