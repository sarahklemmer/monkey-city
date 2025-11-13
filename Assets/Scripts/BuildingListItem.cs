using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingListItem : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    
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
        
        // Check immediately after setup if this should be hidden
        CheckIfShouldHide();
        
        // Subscribe to event after buildingData is set
        if (!string.IsNullOrEmpty(buildingData.buildingName) && buildingData.buildingName == "Tree Of Life")
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