using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingInfoPopup : MonoBehaviour
{
    [SerializeField] TMP_Text description;
    [SerializeField] CanvasGroup parent;
    [SerializeField] GameObject addObject;
    [SerializeField] GameObject moveObject;
    [SerializeField] GameObject removeObject;
    [SerializeField] GameObject upgradeObject;
    // buttons
    AddButton add;
    MoveButton move;
    RemoveButton remove;
    UpgradeButton upgrade;
    
    public bool showing { get; private set; }
    BuildingBase currentlyShowing = null;

    public static BuildingInfoPopup instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BuildingInfoPopup on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;

        add = GetComponentInChildren<AddButton>();
        move = GetComponentInChildren<MoveButton>();
        remove = GetComponentInChildren<RemoveButton>();
        upgrade = GetComponentInChildren<UpgradeButton>();

        Assert.IsNotNull(add, "AddButton is not in children!");
        Assert.IsNotNull(move, "MoveButton is not in children!");
        Assert.IsNotNull(remove, "RemoveButton is not in children!");
        Assert.IsNotNull(upgrade, "UpgradeButton is not in children!");
        Assert.IsNotNull(parent, "forgot to assign parent canvasgroup in inspector in BuildingInfoPopup");
    }

    void Start()
    {
        Hide();
    }

    void Update()
    {
        if(BuildingSelector.instance.currentlyViewing == null && currentlyShowing != null) {
            currentlyShowing = null;
            Hide();
            return;
        }

        if(currentlyShowing != BuildingSelector.instance.currentlyViewing) {
            currentlyShowing = BuildingSelector.instance.currentlyViewing;
            ShowWithBuilding(currentlyShowing);
        }
    }

    public void ShowWithBuilding(BuildingBase b)
    {
        GameManagementHeirarchy.instance.MakeInfoLastChild();
        BuildingManagementUi.instance.HideUnlockedBuildings();
        parent.alpha = 1f;
        parent.interactable = true;

        description.text = b.GetDescription() + "\n\n" + b.GetUpgradeText();

        addObject.SetActive(false);
        removeObject.SetActive(false);
        upgradeObject.SetActive(false);
        moveObject.SetActive(false);

        if(b is TreeOfLife)
        {
            upgrade.building = b;
            upgradeObject.SetActive(true);
            return;
        } else
        {
            moveObject.SetActive(true);
            move.building = b;
        }

        if(!b.canNeverBeUpgraded) {
            upgrade.building = b;
            upgradeObject.SetActive(true);
        }

        if(b.canContainMonkeys) {
            add.building = b;
            addObject.SetActive(true);
            remove.building = b;
            removeObject.SetActive(true);
        }

        if(b is ArcherTower && (b as ArcherTower).NeedToSetType()) PickArcherTowerType.Show(b as ArcherTower);
        else PickArcherTowerType.Hide();
    }

    public void Hide()
    {
        GameManagementHeirarchy.instance.MakeBuildingListLastChild();
        parent.alpha = 0f;
        parent.interactable = false;
        BuildingManagementUi.instance.Show();
    }
}