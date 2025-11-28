using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [HideInInspector] public BuildingBase building;
    [SerializeField] Button button;

    void Start()
    {
        Assert.IsNotNull(button, "forgot to assign add button in the inspector!");
        button.onClick.AddListener(() => Click());
    }

    void Update()
    {
        if(building == null || building.canNeverBeUpgraded)
        {
            button.interactable = false;
            return;
        }
        button.interactable = building.CanUpgrade();
    }
    
    public void Click()
    {
        building.AttemptUpgrade();
    }
}
