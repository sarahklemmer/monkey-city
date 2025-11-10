using UnityEngine;
using UnityEngine.Assertions;

public class MonkeySelector : MonoBehaviour
{
    public static MonkeySelector instance;

    MonkeyController currentlySelected = null;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate MonkeySelector on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Update()
    {
        // right-click to move
        if (!Input.GetMouseButtonDown(1) || currentlySelected == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit)) return;

        BuildingBase building = hit.collider.GetComponentInParent<BuildingBase>();

        // if the building we're moving to is the one we're already allocated to just do nothing
        if (building != null && building == currentlySelected.allocation.CurrentBuilding()) return;

        Vector3 p = hit.point;
        p.y = currentlySelected.transform.position.y; // keep y level

        // bound check for walking off da map
        float r = BuildingGrid.instance.GetGridSize() / 2f;
        if (Mathf.Abs(p.x) > r || Mathf.Abs(p.z) > r) return;

        currentlySelected.StartWalkingToPosition(p.x, p.z, building);
    }


    public void Select(MonkeyController m, bool fromClick = true)
    {
        // cant select a monkey during the tutorial when we're not supposed to
        if (Tutorial.instance.tutorialActive && (Tutorial.instance.tutorialStage < 7 || Tutorial.instance.tutorialStage > 9)) return;
        // clicking a monkey that's already selected should just deselect
        if (currentlySelected == m && fromClick)
        {
            Deselect();
            return;
        }

        Deselect();
        currentlySelected = m;
        // m could be null
        if (currentlySelected != null) currentlySelected.EnableGlow();
    }

    public void Deselect()
    {
        if (currentlySelected != null) currentlySelected.DisableGlow();
        currentlySelected = null;
    }

    public void DeselectIfSelected(MonkeyController m)
    {
        if (currentlySelected == m)
        {
            Deselect();
        }
    }
}
