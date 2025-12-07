using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;

[RequireComponent(typeof(CanvasGroup))]
public class BuildingListItem : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI count;

    [SerializeField] Button button;
    [SerializeField] CanvasGroup cg;
    private int cost;

    public BuildingType type {get; private set;}
    public bool initialized {get; private set;} = false;

    void Awake()
    {
        Assert.IsNotNull(button, "button null in listitem");
        Assert.IsNotNull(cg, "button null in listitem");
    }


    public void Setup(BuildingType type)
    {
        Assert.IsFalse(initialized, "initializing building list item again!");
        initialized = true;
        this.type = type;

        PlaceBuildingButton clickHandler = gameObject.AddComponent<PlaceBuildingButton>();
        clickHandler.Initialize(type);
        button.onClick.AddListener(clickHandler.OnClick);
        cost = BuildingTypeToPrice.GetPrice(type);

        costText.text = cost.ToString() + "\nBanana" + (cost != 1 ? "s" : "");
        iconImage.sprite = BuildingToPrefab.GetIcon(type);
    }

    void Update()
    {
        if(!initialized) return;
        bool atCapacity = false;

        bool active = BuildingTypeToPrice.GetPrice(type) <= BananaManager.instance.GetBananas();
        cg.alpha = active ? 1f : 0.2f;
        cg.interactable = active;

        count.gameObject.SetActive(true);

        int buildingCount = BuildingManager.instance.GetBuildingsOfType(type).Count;
        count.text = buildingCount.ToString() + (type == BuildingType.TreeOfLife || type == BuildingType.Library ? "/1" : "/5");
        
        if((type == BuildingType.TreeOfLife || type == BuildingType.Library) && buildingCount >= 1) {
            count.color = Color.red;
            atCapacity = true;
        }
        else if(buildingCount >= 5) {
            count.color = Color.red;
            atCapacity = true;
        }
        else count.color = Color.black;
        
        button.interactable = !atCapacity && BananaManager.instance.GetBananas() >= cost;
    }
}