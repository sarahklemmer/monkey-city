using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    [HideInInspector] public BuildingBase building;
    [SerializeField] Button button;

    void Start()
    {
        Assert.IsNotNull(button, "forgot to assign move button in the inspector!");
        button.onClick.AddListener(() => Click());
    }

    public void Click()
    {
        MoveButtonOnClick.ClickHandler(building);
    }
}
