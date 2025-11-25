using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class MonkeyController : MonoBehaviour
{
    GlowEffect glow;
    private Coroutine moveRoutine;
    public MonkeyAlloc allocation { get; private set; }

    [SerializeField] private Material outlineMaterial;

    void Awake()
    {
        glow = gameObject.AddComponent<GlowEffect>();
        glow.Initialize(outlineMaterial);
        allocation = gameObject.AddComponent<MonkeyAlloc>();
        allocation.Initialize(this);
    }

    public void StartWalkingToBuilding(BuildingBase buildingTarget)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(WalkToBuilding(buildingTarget));
    }

    private IEnumerator WalkToBuilding(BuildingBase buildingTarget)
    {
        Assert.IsNotNull(buildingTarget);
        Vector3 target = new Vector3(buildingTarget.transform.position.x, transform.position.y, buildingTarget.transform.position.z);
        // always deallocate on walk, either we're leaving a building or we're not allocated and it just
        // does nothing
        allocation.Unassign();
        buildingTarget.monkeys.StartWalking(this);
        // we'll be walking ianto buildings anyways so this 0.05 is fine i think
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            Vector3 direction = target - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }

            transform.position = Vector3.MoveTowards(transform.position, target, AllMonkeyInfo.instance.GetMonkeySpeed() * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        moveRoutine = null;

        // if alloc fails just walk back home type shit
        if (!allocation.Assign(buildingTarget)) StartWalkingToBuilding(BuildingManager.instance.GetTreeOfLife());
    }
    
    public void EnableGlow()  => glow.SetGlow(true);
    public void DisableGlow() => glow.SetGlow(false);
}
