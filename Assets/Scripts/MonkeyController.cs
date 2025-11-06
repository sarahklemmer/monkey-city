using System.Collections;
using UnityEngine;

public class MonkeyController : MonoBehaviour
{
    GlowEffect glow;
    private Coroutine moveRoutine;

    [SerializeField] private float speed = 3f;
    [SerializeField] private Material outlineMaterial;

    void Awake()
    {
        glow = gameObject.AddComponent<GlowEffect>();
        glow.Initialize(outlineMaterial);
    }

    public void StartWalkingToPosition(float x, float z)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(WalkToPosition(new Vector3(x, transform.position.y, z)));
    }

    private IEnumerator WalkToPosition(Vector3 target)
    {
        // we'll be walking into buildings anyways so this 0.05 is fine i think
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        moveRoutine = null;
    }

    void OnMouseDown()
    {
        glow.SetGlow(true);
        MonkeySelector.instance.Select(this);
    }
    
    public void EnableGlow()  => glow.SetGlow(true);
    public void DisableGlow() => glow.SetGlow(false);
}
