using UnityEngine;
using UnityEngine.UI;

public class TreeOfLifeButtons : MonoBehaviour
{
    [SerializeField] GameObject pathButtonObj;
    Button pathButton;
    TreeOfLife tree = null;

    void Start()
    {
        pathButtonObj.SetActive(false);
        pathButton = pathButtonObj.GetComponent<Button>();
        pathButton.interactable = false;
    }

    void Update()
    {
        if (tree == null)
        {
            BuildingBase b = BuildingManager.instance.GetTreeOfLife();
            if(b == null) return;
            tree = b as TreeOfLife;
            pathButtonObj.SetActive(true);
        }

        pathButton.interactable = tree != null && PathManager.instance.pathSelected;
    }
}