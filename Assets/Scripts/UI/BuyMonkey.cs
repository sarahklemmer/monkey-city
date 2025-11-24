using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyMonkey : MonoBehaviour
{
    [SerializeField] int basePrice = 1;
    [SerializeField] TMP_Text priceText;
    [SerializeField] Image monkeyIcon;
    int price;
    void Start()
    {
        price = basePrice;
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
        priceText.text = "$" + price;
        bool interactable = BananaManager.instance.GetBananas() >= price && BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm).Count > 0;
        GetComponent<Button>().interactable = interactable;
        var c = monkeyIcon.color;
        c.a = interactable ? 1f : 0.5f;
        monkeyIcon.color = c;
    }
}
