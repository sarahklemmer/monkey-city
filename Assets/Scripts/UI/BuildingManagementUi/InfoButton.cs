// using UnityEngine;
// using UnityEngine.Assertions;
// using UnityEngine.UI;

// public class InfoButton : MonoBehaviour
// {
//     [HideInInspector] public BuildingBase building;
//     [SerializeField] Button button;

//     void Start()
//     {
//         Assert.IsNotNull(button, "forgot to assign info button in the inspector!");
//         button.onClick.AddListener(() => Click());
//     }

//     public void Click()
//     {
//         BuildingInfo.instance.Show(building);
//     }
// }
