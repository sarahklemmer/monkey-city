using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class BuildingManagementUi : MonoBehaviour
{
    [SerializeField] GameObject BuildingUIElement;
    //TODO: get rid of this and add a script that just sets the icon based on building type
    [SerializeField] Sprite archerTowerIcon;
    [SerializeField] Sprite spikeTrapIcon;

    private readonly Dictionary<BuildingBase, GameObject> uiLookup = new();
    private Transform content;

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
    }

    void Start()
    {
        Assert.IsNotNull(BuildingUIElement, "BuildingUIElement prefab is not assigned!");

        Assert.IsFalse(BuildingUIElement.activeSelf, "BuildingUIElement prefab must be disabled in the inspector!");

        ScrollRect scroll = GetComponentInChildren<ScrollRect>();
        Assert.IsNotNull(scroll, "BuildingManagementUi requires a ScrollRect child.");

        content = scroll.content;
        Assert.IsNotNull(content, "ScrollRect.content is null!");

    }

    public void AddBuilding(BuildingBase building)
    {
        
        Assert.IsNotNull(building);
        if(building is TreeOfLife) return;
        Assert.IsFalse(uiLookup.ContainsKey(building), $"Building {building.name} already has a UI element!");

        GameObject ui = Instantiate(BuildingUIElement, content);
        ui.SetActive(true);

        uiLookup.Add(building, ui);

        AssignBuildingToButtons(ui, building);

        //TODO: set icon for building type more robustly
        if(building.GetBuildingType() == BuildingType.ArcherTower) ui.transform.Find("Icon").GetComponent<Image>().sprite = archerTowerIcon;
        else if(building.GetBuildingType() == BuildingType.SpikeTrap) ui.transform.Find("Icon").GetComponent<Image>().sprite = spikeTrapIcon;

        ReorderByType();
    }

    public void RemoveBuilding(BuildingBase building)
    {
        Assert.IsNotNull(building);
        if(building is TreeOfLife) return;

        Assert.IsTrue(uiLookup.ContainsKey(building), $"Attempted to remove building UI for {building.name}, but none exists!");

        Destroy(uiLookup[building]);
        uiLookup.Remove(building);

        ReorderByType();
    }

    private void AssignBuildingToButtons(GameObject ui, BuildingBase building)
    {
        AddButton add = ui.GetComponentInChildren<AddButton>(true);
        RemoveButton remove = ui.GetComponentInChildren<RemoveButton>(true);
        
        InfoButton info = ui.GetComponentInChildren<InfoButton>(true);
        MoveButton move = ui.GetComponentInChildren<MoveButton>(true);
        
        UpgradeButton upgrade = ui.GetComponentInChildren<UpgradeButton>(true);
        SelectBuildingOnHover selectionEffect = ui.GetComponent<SelectBuildingOnHover>();
        
        Assert.IsNotNull(info, "InfoButton missing from prefab!");
        Assert.IsNotNull(move, "MoveButton missing from prefab!");
        
        Assert.IsNotNull(upgrade, "UpgradeButton missing from prefab!");
        Assert.IsNotNull(selectionEffect, "SelectBuildingOnhover missing from prefab!");

        Assert.IsNotNull(add, "AddButton missing from prefab!");
        Assert.IsNotNull(remove, "RemoveButton missing from prefab!");
        
        info.building = building;
        move.building = building;
        
        upgrade.building = building;
        selectionEffect.building = building;

        if(building.canContainMonkeys)
        {
            add.building = building;
            remove.building = building;
        } else
        {
            add.PermanentlyRemoveButton();
            remove.PermanentlyRemoveButton();
        }
    }

    private void ReorderByType()
    {
        var ordered = uiLookup
            .OrderBy(pair => pair.Key.GetBuildingType()) // relies on enum ordering
            .ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].Value.transform.SetSiblingIndex(i);
        }
    }
}