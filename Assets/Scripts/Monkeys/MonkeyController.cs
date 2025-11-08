using System.Collections;
using UnityEngine;

public class MonkeyController : MonoBehaviour
{
    GlowEffect glow;
    private Coroutine moveRoutine;
    public MonkeyAlloc allocation { get; private set; }

    [SerializeField] private float speed = 3f;
    [SerializeField] private Material outlineMaterial;

    void Awake()
    {
        glow = gameObject.AddComponent<GlowEffect>();
        glow.Initialize(outlineMaterial);
        allocation = gameObject.AddComponent<MonkeyAlloc>();
        allocation.Initialize(this);
    }

    public void StartWalkingToPosition(float x, float z, BuildingBase buildingTarget = null)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(WalkToPosition(new Vector3(x, transform.position.y, z), buildingTarget));
    }

    private IEnumerator WalkToPosition(Vector3 target, BuildingBase buildingTarget)
    {
        // always deallocate on walk, either we're leaving a building or we're not allocated and it just
        // does nothing
        allocation.Deallocate();
        // we'll be walking ianto buildings anyways so this 0.05 is fine i think
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        moveRoutine = null;

        // if there's a targeted building allocate to it
        if(buildingTarget != null) allocation.Allocate(buildingTarget);
    }

    void OnMouseDown()
    {
        glow.SetGlow(true);
        MonkeySelector.instance.Select(this);
    }
    
    public void EnableGlow()  => glow.SetGlow(true);
    public void DisableGlow() => glow.SetGlow(false);
}
