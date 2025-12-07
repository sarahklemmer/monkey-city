using UnityEngine;
using UnityEngine.UI;

public class PickArcherTowerType : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] Button sprayer;
    [SerializeField] Button sniper;
    static PickArcherTowerType instance;
    ArcherTower activeBuilding = null;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate PickArcherTowerType on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Update()
    {
        if(activeBuilding == null) return;
        instance.sniper.enabled = instance.activeBuilding.NeedToSetType() && BananaManager.instance.GetBananas() >= activeBuilding.GetUpgradeCost();
        instance.sprayer.enabled = instance.activeBuilding.NeedToSetType() && BananaManager.instance.GetBananas() >= activeBuilding.GetUpgradeCost();
    }

    void Start()
    {
        Hide();
    }

    public static void Show(ArcherTower archer)
    {
        instance.activeBuilding = archer;
        instance.parent.SetActive(true);
        instance.sprayer.onClick.RemoveAllListeners();
        instance.sprayer.onClick.AddListener(() => {
            archer.SelectType(ArcherTowerType.TackSprayer);
            archer.AttemptUpgrade();
            Hide();    
        });

        instance.sniper.onClick.RemoveAllListeners();
        instance.sniper.onClick.AddListener(() => {
            archer.SelectType(ArcherTowerType.SniperMonkey);
            archer.AttemptUpgrade();
            Hide();    
        });
    }

    public static void Hide() { 
        instance.activeBuilding = null;
        instance.parent.SetActive(false); 
    }
}
