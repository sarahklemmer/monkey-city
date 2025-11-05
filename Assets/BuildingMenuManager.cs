using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject buildingScrollbar;
    private Toggle buildingBarToggle;

    void OpenBuildingBar()
    {
        buildingScrollbar.SetActive(true);
    }

    void CloseBuildingBar()
    {
        buildingScrollbar.SetActive(false);
    }
}
