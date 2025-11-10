using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    public static BuildingSelector instance;

    BuildingBase currentlySelected = null;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BuildingSelector on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void OnMouseDown()
    {
        // Deselect when clicking empty space
        Deselect();
    }

    public void Select(BuildingBase b)
    {
        if (b == null) return;
        
        // From develop: Check if building is selectable
        if (!b.selectable) return;
        
        // // Double clicking a building should deselect it
        // if (currentlySelected == b)
        // {
        //     Deselect();
        //     return;
        // }

        // Deselect previous building
        Deselect();

        if (Tutorial.instance.tutorialActive)
        {
            if (Tutorial.instance.tutorialStage == 5 && b.GetBuildingType() == BuildingType.BananaFarm) Tutorial.instance.PlayerSelectsBananaFarm();
            else if (b.GetBuildingType() != BuildingType.ArcherTower && !(Tutorial.instance.tutorialStage == 8 && b.GetBuildingType() == BuildingType.BananaFarm)) return;
        }

        // Select new building
        currentlySelected = b;
        // currentlySelected.EnableGlow();
        
        // Show building info
        BuildingInfo.instance.Show(currentlySelected);
        
        // From develop: Select monkey in building for allocation/deallocation
        // The false parameter means this is an internal call to avoid the double click deselect check
        MonkeySelector.instance.Select(b.NextMonkeyToRemove(), false);
    }

    public void Deselect()
    {
        if (Tutorial.instance.tutorialActive && Tutorial.instance.tutorialStage == 6) Tutorial.instance.PlayerClosesBananaFarmWindow();
        // Hide building info
        BuildingInfo.instance.Hide();
        
        if (currentlySelected != null)
        {
            currentlySelected.DisableGlow();
        }
        
        currentlySelected = null;
    }
}