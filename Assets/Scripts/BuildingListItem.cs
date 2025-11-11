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
    
    public void Setup(BuildingData data)
    {
        buildingData = data;
        
        if (iconImage != null)
            iconImage.sprite = data.icon;
            
        if (nameText != null)
            nameText.text = data.buildingName;
            
        if (costText != null)
            costText.text = "$" + data.cost.ToString();
            
        CheckIfShouldHide();
    }
    
    private void CheckIfShouldHide()
    {
        if (buildingData.buildingName == "Tree of Life")
        {
            TreeOfLife existingTree = FindFirstObjectByType<TreeOfLife>();
            
            if (existingTree != null)
            {
                gameObject.SetActive(false);
            }
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