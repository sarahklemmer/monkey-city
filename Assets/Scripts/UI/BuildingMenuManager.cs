using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct BuildingData
{
    public string buildingName;
    public Sprite icon;
    public int cost;
    public string description;
    public BuildingType type;
}

public class BuildingMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject buildingScrollbar;
    [SerializeField] private ScrollableList scrollableList;
    [SerializeField] private Transform disableInteractivityRoot;

    public BuildingData[] allBuildings;
    public Toggle toggle;
    
    public Image background;
    
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color selectedColor = new Color(1f, 0.9f, 0.5f, 1f);

    public static BuildingMenuManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BuildingMenuManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        toggle.isOn = false;
        toggle.onValueChanged.AddListener((isOn) => OnToggle(isOn));

        HideAllMenus();
    }
    
    BuildingData[] GetUnlockedBuildings() => allBuildings.Where(b => BuildingUnlock.Unlocked(b.type)).ToArray();
 
    void OnToggle(bool isOn)
    {
        background.color = isOn ? selectedColor : normalColor;

        if (isOn)
        {
            if(disableInteractivityRoot != null) UIInteractabilityManager.instance.DisableInteractivityExcept(disableInteractivityRoot, true);
            buildingScrollbar.SetActive(true);
            scrollableList.PopulateList(GetUnlockedBuildings());

            BuildingGrid.instance.DestroyBuildingPlacementIndicators();
        }
        else
        {
            if(disableInteractivityRoot != null) UIInteractabilityManager.instance.EnableInteractivity();
            buildingScrollbar.SetActive(false);
            scrollableList.ClearList();
        }
    }
    
    public void RefreshMenu()
    {
        if (toggle.isOn && buildingScrollbar.activeSelf)
        {
            scrollableList.PopulateList(GetUnlockedBuildings());
        }
    }
    
    public void ForceCloseMenu()
    {
        toggle.isOn = false;
        buildingScrollbar.SetActive(false);
        scrollableList.ClearList();
    }

    public void HideAllMenus()
    {
        buildingScrollbar.SetActive(false);
        scrollableList.ClearList();
        
        background.color = normalColor;
    }

    public void CloseAllMenus()
    {
        toggle.isOn = false;
        HideAllMenus();
    }
    
    public void UpdatePrices()
    {
        scrollableList.PopulateList(GetUnlockedBuildings());
    }
}