using UnityEngine;
using UnityEngine.UI;

public class SetToBuildingIcon : MonoBehaviour
{
    [SerializeField] Image buildingIcon;
    [SerializeField] Image typeIcon;
    [SerializeField] Sprite bananaSprite;
    [SerializeField] Sprite defenseSprite;
    [SerializeField] Sprite monkeySprite;

    void Update()
    {
        BuildingBase b = BuildingSelector.instance.currentlySelected;

        if(b == null) return;

        buildingIcon.sprite = BuildingToPrefab.GetIcon(b.type);
        
        if(b.type == BuildingType.TreeOfLife) typeIcon.sprite = monkeySprite;
        else if(b.type == BuildingType.BananaFarm) typeIcon.sprite = bananaSprite;
        else typeIcon.sprite = defenseSprite;
    }
}
