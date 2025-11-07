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
        // hide old building info
        BuildingInfo.instance.Hide();

        if (currentlySelected == b)
        {
            // toggle off
            currentlySelected.DisableGlow();
            currentlySelected = null;
            return;
        }

        if (currentlySelected != null) currentlySelected.DisableGlow();
        currentlySelected = b;
        currentlySelected.EnableGlow();
        // show new building info
        BuildingInfo.instance.Show(currentlySelected);
    }
}
