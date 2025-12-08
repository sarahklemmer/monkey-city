using UnityEngine;
using UnityEngine.UI;

public class PickArcherTowerType : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] Button sprayer;
    [SerializeField] Button sniper;
    static PickArcherTowerType instance;

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

    void Start()
    {
        Hide();
    }

    public static void Show(ArcherTower archer)
    {
        instance.parent.SetActive(true);
 
        instance.sprayer.onClick.RemoveAllListeners();
        instance.sprayer.onClick.AddListener(() => {
            archer.SelectType(ArcherTowerType.TackSprayer);
            Hide();    
        });

        instance.sniper.onClick.RemoveAllListeners();
        instance.sniper.onClick.AddListener(() => {
            archer.SelectType(ArcherTowerType.SniperMonkey);
            Hide();    
        });
    }

    public static void Hide() { 
        instance.parent.SetActive(false); 
    }
}
