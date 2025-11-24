using UnityEngine;
using UnityEngine.UI;

public class TreeOfLifeButtons : MonoBehaviour
{
    [SerializeField] GameObject pathButtonObj;
    [SerializeField] GameObject upgradeButtonObj;
    Button upgradeButton;
    Button pathButton;

    const int TREE_OF_LIFE_UPGRADE_COST = 100;

    TreeOfLife tree;
    bool listenerAdded = false;

    void Start()
    {
        pathButtonObj.SetActive(false);
        upgradeButtonObj.SetActive(false);
        upgradeButton = upgradeButtonObj.GetComponent<Button>();
        pathButton = pathButtonObj.GetComponent<Button>();
        upgradeButton.interactable = false;
        pathButton.interactable = false;
    }

    void Update()
    {
        if (tree == null)
        {
            BuildingBase b = BuildingManager.instance.GetTreeOfLife();
            if(b == null) return;
            tree = b as TreeOfLife;
        }

        upgradeButtonObj.SetActive(true);
        pathButtonObj.SetActive(true);

        if (!listenerAdded)
        {
            listenerAdded = true;
            upgradeButton.onClick.AddListener(UpgradeTreeOfLife);
        }

        upgradeButton.interactable =
            tree != null &&
            !tree.IsMaxLevel() &&
            BananaManager.instance.GetBananas() >= TREE_OF_LIFE_UPGRADE_COST;

        pathButton.interactable =
            tree != null &&
            PathManager.instance.pathSelected;
    }

    void UpgradeTreeOfLife()
    {
        if (tree == null) return;
        if (tree.IsMaxLevel() || BananaManager.instance.GetBananas() < TREE_OF_LIFE_UPGRADE_COST) return;

        BananaManager.instance.AddBananas(-TREE_OF_LIFE_UPGRADE_COST);
        tree.Upgrade();
        BuildingInfo.instance.PlayUpgradeEffect(tree);
    }
}