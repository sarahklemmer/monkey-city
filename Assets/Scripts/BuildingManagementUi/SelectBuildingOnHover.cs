using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class SelectBuildingOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public BuildingBase building;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Assert.IsNotNull(building, "SelectBuildingOnHover missing building reference!");
        BuildingSelector.instance.Select(building);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Assert.IsNotNull(building, "SelectBuildingOnHover missing building reference!");
        BuildingSelector.instance.DeselectSpecificBuilding(building);
    }
}