using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct BuildingData
{
    public string buildingName;
    public Sprite icon;
    public int cost;
    public string description;
}

public class BuildingMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject buildingScrollbar;
    [SerializeField] private ScrollableList scrollableList; // Changed from UIElements.ScrollView

    [Header("Building Data Lists")]
    public BuildingData[] bananaBuildings;
    public BuildingData[] defenseBuildings;
    public BuildingData[] happyBuildings;

    [Header("Category Toggles")]
    public Toggle bananaToggle;
    public Toggle defenseToggle;
    public Toggle happyToggle;
    
    [Header("Toggle Colors")]
    public Image bananaBackground;
    public Image defenseBackground;
    public Image happyBackground;
    
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Light gray
    public Color selectedColor = new Color(1f, 0.9f, 0.5f, 1f); // Yellow sheen
    
    void Start()
    {
        bananaToggle.isOn = false;
        defenseToggle.isOn = false;
        happyToggle.isOn = false;
        
        bananaToggle.onValueChanged.AddListener((isOn) => OnBananaToggle(isOn));
        defenseToggle.onValueChanged.AddListener((isOn) => OnDefenseToggle(isOn));
        happyToggle.onValueChanged.AddListener((isOn) => OnHappyToggle(isOn));

        HideAllMenus();
    }
    
    void OnBananaToggle(bool isOn)
    {
        bananaBackground.color = isOn ? selectedColor : normalColor;
        
        if (isOn)
        {
            // Turn off other toggles
            defenseToggle.isOn = false;
            happyToggle.isOn = false;
            
            // Show scrollbar and populate with banana buildings
            buildingScrollbar.SetActive(true);
            scrollableList.PopulateList(bananaBuildings);
        }
        else
        {
            // Hide scrollbar and clear list
            buildingScrollbar.SetActive(false);
            scrollableList.ClearList();
        }
    }
    
    void OnDefenseToggle(bool isOn)
    {
        defenseBackground.color = isOn ? selectedColor : normalColor;
        
        if (isOn)
        {
            // Turn off other toggles
            bananaToggle.isOn = false;
            happyToggle.isOn = false;
            
            // Show scrollbar and populate with defense buildings
            buildingScrollbar.SetActive(true);
            scrollableList.PopulateList(defenseBuildings);
        }
        else
        {
            // Hide scrollbar and clear list
            buildingScrollbar.SetActive(false);
            scrollableList.ClearList();
        }
    }
    
    void OnHappyToggle(bool isOn)
    {
        happyBackground.color = isOn ? selectedColor : normalColor;
        
        if (isOn)
        {
            // Turn off other toggles
            bananaToggle.isOn = false;
            defenseToggle.isOn = false;
            
            // Show scrollbar and populate with happy buildings
            buildingScrollbar.SetActive(true);
            scrollableList.PopulateList(happyBuildings);
        }
        else
        {
            // Hide scrollbar and clear list
            buildingScrollbar.SetActive(false);
            scrollableList.ClearList();
        }
    }
    
    void HideAllMenus()
    {
        buildingScrollbar.SetActive(false);
        scrollableList.ClearList();
        
        bananaBackground.color = normalColor;
        defenseBackground.color = normalColor;
        happyBackground.color = normalColor;
    }
    
    // Call this from your X/close button
    public void CloseAllMenus()
    {
        bananaToggle.isOn = false;
        defenseToggle.isOn = false;
        happyToggle.isOn = false;
        HideAllMenus();
    }
}