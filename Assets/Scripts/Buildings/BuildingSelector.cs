using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    public static BuildingSelector instance;
    [SerializeField] LayerMask buildingMask;
    private bool selectionDisabled = false;
    private bool selectionDisabledForFrame = false;

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

    void Update()
    {
        if(selectionDisabledForFrame)
        {
            selectionDisabledForFrame = false;
            return;
        } else if(selectionDisabled) {
            return;
        }
        // neither mouse button pressed
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if (!leftClick && !rightClick) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        BuildingBase clicked = null;
        if (Physics.Raycast(ray, out hit, 1000f, buildingMask))
            clicked = hit.collider.GetComponentInParent<BuildingBase>();
        
        // right click, building selected, and right clicked another building
        if (rightClick && clicked != null && currentlySelected != null)
        {   
            if (clicked == currentlySelected) return; // ignore if same building
            if (!clicked.CanAllocate()) return;       // building is full or unavailable

            // remove next monkey and send them to the building we clicked
            MonkeyController victim = currentlySelected.NextMonkeyToRemove();
            // no monkeys 
            if (victim == null) return;
            currentlySelected.RemoveMonkey(victim);
            victim.StartWalkingToBuilding(clicked);
        }
        // left click for selecting
        else if (leftClick)
        {
            if (clicked == null) Deselect();
            // shift click shows info
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                BuildingInfo.instance.Show(clicked);
                return;
            }
            // select if we didn't JUST place something
            else Select(clicked);
        }
    }

    public void Select(BuildingBase b)
    {
        if (b == null) return;
        // deselect previous building
        Deselect();
        // we still want to deselect even if b isn't selectable as it should be like any other random press on scenery
        if (!b.selectable) return;

        // Select new building
        currentlySelected = b;
        currentlySelected.EnableGlow();
    }

    public void Deselect()
    {
        if (currentlySelected != null)
        {
            currentlySelected.DisableGlow();
        }

        currentlySelected = null;
    }

    public void DeselectSpecificBuilding(BuildingBase b)
    {
        if(currentlySelected == b) Deselect();
    }

    public void DisableSelectionThisFrame()
    {
        selectionDisabledForFrame = true;
    }

    public void DisableSelection() { 
        selectionDisabled = true; 
        Deselect();
    }

    public void EnableSelection()  {
        selectionDisabled = false;
        
    }

    public bool BuildingSelected() => currentlySelected == null;
}