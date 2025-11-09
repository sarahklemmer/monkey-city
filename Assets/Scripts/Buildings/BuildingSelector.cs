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
        
        // Double clicking a building should deselect it
        if (currentlySelected == b)
        {
            Deselect();
            return;
        }

        // Deselect previous building
        Deselect();

        // Select new building
        currentlySelected = b;
        currentlySelected.EnableGlow();
        
        // Show building info
        BuildingInfo.instance.Show(currentlySelected);
        
        // From develop: Select monkey in building for allocation/deallocation
        // The false parameter means this is an internal call to avoid the double click deselect check
        MonkeySelector.instance.Select(b.NextMonkeyToRemove(), false);
    }

    public void Deselect()
    {
        // Hide building info
        BuildingInfo.instance.Hide();
        
        if (currentlySelected != null)
        {
            currentlySelected.DisableGlow();
        }
        
        currentlySelected = null;
    }

    public BuildingBase Selected() => currentlySelected;
}