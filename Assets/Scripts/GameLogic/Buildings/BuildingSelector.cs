using UnityEngine;

public class BuildingSelector : MonoBehaviour
{
    public static BuildingSelector instance;
    [SerializeField] LayerMask buildingMask;
    [SerializeField] RectTransform uiSafeArea;
    [SerializeField] private TutorialManager tutorialManager;
    private bool selectionDisabled = false;
    private bool selectionDisabledForFrame = false;
    private bool lastStep = false;

    public BuildingBase currentlySelected {get; private set;} = null;
    public BuildingBase currentlyViewing {get; private set;} = null;
    MonkeyController selectedMonkey = null;
    
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

        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        if (!leftClick && !rightClick) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        BuildingBase clicked = null;
        if (Physics.Raycast(ray, out hit, 1000f, buildingMask))
            clicked = hit.collider.GetComponentInParent<BuildingBase>();
        
        // if we right click 
        if (rightClick && clicked != null)
        {
            currentlyViewing = clicked;
            return;
        } else if (rightClick)
        {
            currentlyViewing = null;
        }

        // we know it was a leftclick from here
        
        if (clicked != null)
        {
            if (currentlySelected != null && selectedMonkey != null)
            {
                if (clicked != currentlySelected && clicked.CanAllocate())
                {
                    currentlySelected.RemoveMonkey(selectedMonkey);
                    selectedMonkey.StartWalkingToBuilding(clicked);
                    Deselect();
                    return;
                }
            }

            Select(clicked);
        }
        else
        {
            // if we clicked not on the UI
            if (!RectTransformUtility.RectangleContainsScreenPoint(uiSafeArea, Input.mousePosition))
            {
                currentlyViewing = null;
                Deselect();
            }
        }
    }

    public void Select(BuildingBase b)
    {
        if (b == null) return;
        
        Deselect();
        
        if (!b.selectable) return;

        currentlySelected = b;
        currentlySelected.EnableGlow();

        if (currentlySelected.GetMonkeyCount() > 0)
        {
            selectedMonkey = currentlySelected.monkeys.MonkeyToDeallocate();
            if (selectedMonkey != null)
            {
                selectedMonkey.EnableGlow();
            }
        }
        
        if (tutorialManager.isActive && tutorialManager.currentStepIndex == 4)
        {
            Debug.Log($"Tutorial active, completing step {tutorialManager.currentStepIndex}");
            tutorialManager.OnStepCompleted();
        }
        Debug.Log($"isActive: {tutorialManager.isActive}, currentStepIndex: {tutorialManager.currentStepIndex}");
    }

    public void Deselect()
    {
        if (currentlySelected != null)
        {
            currentlySelected.DisableGlow();
        }

        if (selectedMonkey != null)
        {
            selectedMonkey.DisableGlow();
            selectedMonkey = null;
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