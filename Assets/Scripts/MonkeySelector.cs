using UnityEngine;

public class MonkeySelector : MonoBehaviour
{
    [SerializeField] LayerMask groundMask = default;
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
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundMask))
        {
            Vector3 p = hit.point;
            p.y = currentlySelected.transform.position.y; // stay on plane

            float r = BuildingGrid.instance.GetGridSize() / 2f;
            if (Mathf.Abs(p.x) > r || Mathf.Abs(p.z) > r) return;

            currentlySelected.StartWalkingToPosition(p.x, p.z);
        }
    }


    public void Select(MonkeyController m)
    {
        if (currentlySelected == m)
        {
            // toggle off
            currentlySelected.DisableGlow();
            currentlySelected = null;
            return;
        }

        if (currentlySelected != null) currentlySelected.DisableGlow();
        currentlySelected = m;
        currentlySelected.EnableGlow();
    }
}
