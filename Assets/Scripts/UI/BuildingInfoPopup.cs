using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingInfoPopup : MonoBehaviour
{
    [SerializeField] TMP_Text description;
    [SerializeField] CanvasGroup parent;
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

        add = parent.GetComponentInChildren<AddButton>();
        move = parent.GetComponentInChildren<MoveButton>();
        remove = parent.GetComponentInChildren<RemoveButton>();
        upgrade = parent.GetComponentInChildren<UpgradeButton>();

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
        if(BuildingSelector.instance.currentlySelected == null && currentlyShowing != null) {
            currentlyShowing = null;
            Hide();
            return;
        }

        if(currentlyShowing != BuildingSelector.instance.currentlySelected) {
            currentlyShowing = BuildingSelector.instance.currentlySelected;
            ShowWithBuilding(currentlyShowing);
        }
    }

    public void ShowWithBuilding(BuildingBase b)
    {
        BuildingManagementUi.instance.HideUnlockedBuildings();
        parent.alpha = 1f;
        parent.interactable = true;

        description.text = b.GetDescription() + "\n\n" + b.GetUpgradeText();

        // move is always active 
        add.gameObject.SetActive(false);
        remove.gameObject.SetActive(false);
        upgrade.gameObject.SetActive(false);

        if(!b.canNeverBeUpgraded) {
            upgrade.building = b;
            upgrade.gameObject.SetActive(true);
        }

        if(b.canContainMonkeys) {
            add.building = b;
            remove.building = b;

            add.gameObject.SetActive(true);
            remove.gameObject.SetActive(true);
        }

        move.building = b;

        if(b is ArcherTower && (b as ArcherTower).NeedToSetType()) PickArcherTowerType.Show(b as ArcherTower);
        else PickArcherTowerType.Hide();
    }

    public void Hide()
    {
        parent.alpha = 0f;
        parent.interactable = false;
        BuildingManagementUi.instance.Show();
    }
}
