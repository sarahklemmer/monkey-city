using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject buildingScrollbar;
    [SerializeField] private GameObject buildingButtonPrefab; 
//    [SerializeField] private ScrollView scrollableList;

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
        buildingScrollbar.SetActive(isOn);
    }
    
    void OnDefenseToggle(bool isOn)
    {
        defenseBackground.color = isOn ? selectedColor : normalColor;
        buildingScrollbar.SetActive(isOn);
    }
    
    void OnHappyToggle(bool isOn)
    {
        happyBackground.color = isOn ? selectedColor : normalColor;
        buildingScrollbar.SetActive(isOn);
    }
    
    void HideAllMenus()
    {
        buildingScrollbar.SetActive(false);
        
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

    void OpenBuildingBar()
    {
        buildingScrollbar.SetActive(true);
    }

    void CloseBuildingBar()
    {
        buildingScrollbar.SetActive(false);
    }
}
