using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyMonkey : MonoBehaviour
{
    [SerializeField] int basePrice = 1;
    [SerializeField] TMP_Text priceText;
    [SerializeField] Image monkeyIcon;
    [SerializeField] GameObject bananaPrefab;
    [SerializeField] Transform bananaIconParent;
    
    int price;
    private GameObject currentBananaIcon;
    
    void Awake()
    {
    }
    
    void Start()
    {
        price = basePrice;
        if (bananaPrefab != null && bananaIconParent != null)
        {
            Debug.Log("[BuyMonkey] Attempting to instantiate banana icon...");
            currentBananaIcon = Instantiate(bananaPrefab, bananaIconParent);
            Debug.Log($"[BuyMonkey] Instantiated: {currentBananaIcon.name}");
            
            RectTransform rt = currentBananaIcon.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.localScale = Vector3.one;
                rt.anchoredPosition = Vector2.zero;
                Debug.Log($"[BuyMonkey] Banana icon spawned at {rt.anchoredPosition}, active: {currentBananaIcon.activeInHierarchy}");
            }
            else
            {
                Debug.LogWarning("[BuyMonkey] Banana prefab doesn't have a RectTransform! It won't show in UI.");
            }
        }
        else
        {
            if (bananaPrefab == null) Debug.LogWarning("[BuyMonkey] Banana prefab not assigned!");
            if (bananaIconParent == null) Debug.LogWarning("[BuyMonkey] Banana icon parent not assigned!");
        }
    }

    public void Click()
    {
        if(BananaManager.instance.GetBananas() < price) return;
        BananaManager.instance.AddBananas(-price);
        PopulationManager.instance.AddToPopulation(1);
        switch(price)
        {
            case 1:
                price = 10;
                return;
            case 10:
                price = 50;
                return;
            case 50:
                price = 250;
                return;
            case 250:
                price = 1000;
                return;
            default:
                price *= 5;
                return;
        }
    }

    void Update()
    {
        priceText.text = price.ToString();
        
        bool interactable = BananaManager.instance.GetBananas() >= price && BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm).Count > 0;
        GetComponent<Button>().interactable = interactable;
        var c = monkeyIcon.color;
        c.a = interactable ? 1f : 0.5f;
        monkeyIcon.color = c;
    }
    
    void OnDestroy()
    {
        if (currentBananaIcon != null)
        {
            Destroy(currentBananaIcon);
        }
    }
}