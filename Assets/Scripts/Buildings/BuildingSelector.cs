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
        //TODO: have monkeys leave
    }

    public void Select(BuildingBase b)
    {
        // double clicking a building should deselect it, and selecting null should just deselect
        if(currentlySelected == b || b == null)
        {
            Deselect();
            return;
        }

        Deselect();

        currentlySelected = b;
        currentlySelected.EnableGlow();
        // show new building info
        BuildingInfo.instance.Show(currentlySelected);
        // select monkey in building so we can move it out, the false means this is an internal call and avoid the double click
        // deselect check
        MonkeySelector.instance.Select(b.NextMonkeyToRemove(), false);
    }

    public void Deselect()
    {
        // hide old building info
        BuildingInfo.instance.Hide();
        if(currentlySelected != null) currentlySelected.DisableGlow();
        currentlySelected = null;
    }

    public BuildingBase Selected() => currentlySelected;
}
