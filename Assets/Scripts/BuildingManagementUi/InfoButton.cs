using UnityEngine;
using UnityEngine.UI;

public class InfoButton : MonoBehaviour
{
    BuildingBase building;
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
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
