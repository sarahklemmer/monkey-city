using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class BuildingManagementUi : MonoBehaviour
{
    [SerializeField] GameObject BuildingUIElement;

    private Transform content;
    private List<GameObject> buildingUIElements = new List<GameObject>();
    private bool buildingPlacementButtonsShowing = false;

    public static BuildingManagementUi instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BuildingManagementUi on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;

        Assert.IsNotNull(BuildingUIElement, "BuildingUIElement prefab is not assigned!");

        ScrollRect scroll = GetComponentInChildren<ScrollRect>();
        Assert.IsNotNull(scroll, "BuildingManagementUi requires a ScrollRect child.");

        content = scroll.content;
        Assert.IsNotNull(content, "ScrollRect.content is null!");
    }

    public void Show()
    {
        if(!buildingPlacementButtonsShowing) DrawUnlockedBuildings();
    }

    public void RequestUpdateUnlockedBuildings()
    {
        if(buildingPlacementButtonsShowing) DrawUnlockedBuildings();
    }

    void DrawUnlockedBuildings()
    {
        buildingPlacementButtonsShowing = true;
        List<BuildingType> buildingsToRender = BuildingUnlock.GetUnlockedBuildings().ToList();

        // i doubt this is necessary but we should avoid changes in the ordering of buildings in the UI
        buildingsToRender.Sort();
        // not using iterators here because we're modifying the list in place
        for (int i = buildingUIElements.Count - 1; i >= 0; i--)
        {
            GameObject uiObj = buildingUIElements[i];
            BuildingListItem item = uiObj.GetComponent<BuildingListItem>();

            if (!BuildingUnlock.Unlocked(item.type))
            {
                Destroy(uiObj);
                buildingUIElements.RemoveAt(i);
            }
        }
        // 1. remove non unlocked buildings from buildingUIElements
        // 2. draw all not unlocked building types
    
        HashSet<BuildingType> displayedTypes = new HashSet<BuildingType>();
        foreach (GameObject uiObj in buildingUIElements)
        {
            displayedTypes.Add(uiObj.GetComponent<BuildingListItem>().type);
        }

        // instatiate for all non unlocked buildings, so set of unlocked buildings - set of displayed types
        HashSet<BuildingType> typesToAdd = new HashSet<BuildingType>(BuildingUnlock.GetUnlockedBuildings());
        typesToAdd.ExceptWith(displayedTypes); 

        foreach (BuildingType type in typesToAdd)
        {
            Debug.Log(type);
            GameObject ui = Instantiate(BuildingUIElement, content);
            ui.SetActive(true);

            Debug.Log(ui.name);
            
            BuildingListItem item = ui.GetComponent<BuildingListItem>();
            item.Setup(type);

            buildingUIElements.Add(ui);
        }

        SortUIList();

        foreach(GameObject elt in buildingUIElements)
        {
            elt.SetActive(true);
        }
    }

    public void HideUnlockedBuildings()
    {
        Debug.Log("hiding");
        buildingPlacementButtonsShowing = false;
        foreach(GameObject elt in buildingUIElements)
        {
            elt.SetActive(false);
        }
    }

    private void SortUIList()
    {
        buildingUIElements.Sort((a, b) => 
        {
            var typeA = a.GetComponent<BuildingListItem>().type;
            var typeB = b.GetComponent<BuildingListItem>().type;
            return typeA.CompareTo(typeB);
        });

        for (int i = 0; i < buildingUIElements.Count; i++) { buildingUIElements[i].transform.SetSiblingIndex(i); }
    }
}