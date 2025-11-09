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
        if (currentlySelected != null)
        {
            currentlySelected.DisableGlow();
            currentlySelected = null;
            BuildingInfo.instance.Hide();
        }
    }

    public void Select(BuildingBase b)
    {
        if (currentlySelected == b)
        {
            currentlySelected.DisableGlow();
            currentlySelected = null;
            BuildingInfo.instance.Hide();
            return;
        }

        if (currentlySelected != null)
        {
            currentlySelected.DisableGlow();
        }

        currentlySelected = b;
        currentlySelected.EnableGlow();
        
        BuildingInfo.instance.Show(currentlySelected);
    }
}