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
        // Get the button component
        button = GetComponent<Button>();
        
        // Hook up the click event
        if (button != null)
        {
            button.onClick.AddListener(OnClick);
        }
    }
    
    // This method fills in all the UI with the building's info
    public void Setup(BuildingData data)
    {
        buildingData = data;
        
        // Fill in the UI elements
        if (iconImage != null)
            iconImage.sprite = data.icon;
            
        if (nameText != null)
            nameText.text = data.buildingName;
            
        if (costText != null)
            costText.text = "$" + data.cost.ToString();
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
}
