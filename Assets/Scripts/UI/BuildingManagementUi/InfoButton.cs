using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    [HideInInspector] public BuildingBase building;
    [SerializeField] Button button;

    void Start()
    {
        Assert.IsNotNull(button, "forgot to assign info button in the inspector!");
        button.onClick.AddListener(() => Click());
    }

    void Update()
    {
        // just for future proofing if you can put all monkeys in a building make it so that getmonkeycount is 
        // less than the total population
        button.interactable = !GlobalInteractionLock.IsLocked();
    }
    
    public void Click()
    {
        BuildingInfo.instance.Show(building);
    }
}
