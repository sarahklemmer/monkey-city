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

    // We track all currently spawned UI elements
    private readonly Dictionary<BuildingBase, GameObject> uiLookup = new();

    // Reference to the scroll UI's content transform
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

        // UI element must not be active (otherwise it flashes onscreen at start)
        Assert.IsFalse(BuildingUIElement.activeSelf,
            "BuildingUIElement prefab must be disabled in the inspector!");

        // content must be the first child of our Viewport
        ScrollRect scroll = GetComponentInChildren<ScrollRect>();
        Assert.IsNotNull(scroll, "BuildingManagementUi requires a ScrollRect child.");

        content = scroll.content;
        Assert.IsNotNull(content, "ScrollRect.content is null!");

    }

    public void AddBuilding(BuildingBase building)
    {
        
        Assert.IsNotNull(building);
        Assert.IsFalse(uiLookup.ContainsKey(building), $"Building {building.name} already has a UI element!");

        // only add banana farms and archer towers
        if(building.GetBuildingType() != BuildingType.ArcherTower && building.GetBuildingType() != BuildingType.BananaFarm) return;

        GameObject ui = Instantiate(BuildingUIElement, content);
        ui.SetActive(true);

        uiLookup.Add(building, ui);

        AssignBuildingToButtons(ui, building);

        //TODO: set icon for building type more robustly
        if(building.GetBuildingType() == BuildingType.ArcherTower) ui.transform.Find("Icon").GetComponent<Image>().sprite = archerTowerIcon;

        ReorderByType();
    }

    public void RemoveBuilding(BuildingBase building)
    {
        Assert.IsNotNull(building);

        // only remove banana farms and archer towers
        if(building.GetBuildingType() != BuildingType.ArcherTower && building.GetBuildingType() != BuildingType.BananaFarm) return;

        Assert.IsTrue(uiLookup.ContainsKey(building),
            $"Attempted to remove building UI for {building.name}, but none exists!");

        Destroy(uiLookup[building]);
        uiLookup.Remove(building);

        ReorderByType();
    }

    private void AssignBuildingToButtons(GameObject ui, BuildingBase building)
    {
        AddButton add = ui.GetComponentInChildren<AddButton>(true);
        InfoButton info = ui.GetComponentInChildren<InfoButton>(true);
        MoveButton move = ui.GetComponentInChildren<MoveButton>(true);
        RemoveButton remove = ui.GetComponentInChildren<RemoveButton>(true);
        UpgradeButton upgrade = ui.GetComponentInChildren<UpgradeButton>(true);
        SelectBuildingOnHover selectionEffect = ui.GetComponent<SelectBuildingOnHover>();

        Assert.IsNotNull(add, "AddButton missing from prefab!");
        Assert.IsNotNull(info, "InfoButton missing from prefab!");
        Assert.IsNotNull(move, "MoveButton missing from prefab!");
        Assert.IsNotNull(remove, "RemoveButton missing from prefab!");
        Assert.IsNotNull(upgrade, "UpgradeButton missing from prefab!");
        Assert.IsNotNull(selectionEffect, "SelectBuildingOnhover missing from prefab!");

        add.building = building;
        info.building = building;
        move.building = building;
        remove.building = building;
        upgrade.building = building;
        selectionEffect.building = building;
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