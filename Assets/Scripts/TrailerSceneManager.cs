using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonkeyCutscene : MonoBehaviour
{
    [Header("Leader Monkey")]
    public Animator leaderAnimator;
    public Transform leaderTransform;

    [Header("Leader Animation Clips")]
    public AnimationClip walkClip;
    public AnimationClip surprisedClip;
    public AnimationClip danceClip;
    public AnimationClip climbClip;

    [Header("Friends")]
    public List<Animator> friendAnimators;
    public List<Transform> friendTransforms;

    [Header("Friends Animation Clips")]
    public AnimationClip friendDanceClip;
    public AnimationClip friendClimbClip;

    [Header("Scene Targets")]
    public Transform walkTarget;
    public Transform treeTransform;

    [Header("Camera")]
    public Camera cutsceneCamera;
    public Transform camStart;
    public Transform camReveal;
    public Transform camTreeFocus;

    [Header("Timings")]
    public float leaderWalkSpeed = 20f;
    public float friendFollowDelay = 1.2f;

    void Start()
    {
        StartCoroutine(RunCutscene());
    }

    IEnumerator RunCutscene()
    {
        // --- Camera Intro ---
        cutsceneCamera.transform.position = camStart.position;
        cutsceneCamera.transform.rotation = camStart.rotation;

        // --- Monkey Walks Backward to Leaves ---
        leaderAnimator.Play(walkClip.name);

        // Move backward to walkTarget
        Vector3 backwardTarget = walkTarget.position;
        yield return MoveTo(leaderTransform, backwardTarget, leaderWalkSpeed);

        // --- Rotate Monkey to face Tree of Life ---
        Vector3 directionToTree = (treeTransform.position - leaderTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTree);
        float rotateTime = 0.5f;
        float elapsed = 0f;
        Quaternion startRotation = leaderTransform.rotation;
        while (elapsed < rotateTime)
        {
            leaderTransform.rotation = Quaternion.Slerp(startRotation, lookRotation, elapsed / rotateTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        leaderTransform.rotation = lookRotation;

        // --- Monkey Calls Friends (They Dance Over) ---
        for (int i = 0; i < friendAnimators.Count; i++)
        {
            StartCoroutine(FriendDanceOver(friendAnimators[i], friendTransforms[i]));
            yield return new WaitForSeconds(friendFollowDelay);
        }

        // --- Camera shifts to Tree ---
        yield return MoveCamera(cutsceneCamera.transform, camTreeFocus, 2f);

        // --- Everyone Climbs the Tree ---
        leaderAnimator.Play(climbClip.name);
        foreach (var friend in friendAnimators)
            friend.Play(friendClimbClip.name);

        // Move toward the tree
        yield return MoveTo(leaderTransform, treeTransform.position, 15f);
        foreach (var t in friendTransforms)
            StartCoroutine(MoveTo(t, treeTransform.position, 15f));

        // --- End of Cutscene ---
        // --- Everyone Climbs the Tree ---
        leaderAnimator.Play(climbClip.name);
        foreach (var friend in friendAnimators)
            friend.Play(friendClimbClip.name);

        // Move leader up the tree
        Vector3 treeTop = treeTransform.position + new Vector3(0, 5f, 0); // adjust 5f to tree height
        yield return MoveTo(leaderTransform, treeTop, 2f);

        // Move friends up the tree with slight offsets
        for (int i = 0; i < friendTransforms.Count; i++)
        {
            Vector3 friendTarget = treeTop + new Vector3(i * 0.5f, 0, i * 0.5f); // stagger them slightly
            StartCoroutine(MoveTo(friendTransforms[i], friendTarget, 2f));
        }
    }


    // Helper: Move Object
    IEnumerator MoveTo(Transform obj, Vector3 target, float speed)
    {
        while (Vector3.Distance(obj.position, target) > 0.1f)
        {
            obj.position = Vector3.MoveTowards(obj.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    // Helper: Camera Move
    IEnumerator MoveCamera(Transform cam, Transform target, float duration)
    {
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cam.position = Vector3.Lerp(startPos, target.position, t);
            cam.rotation = Quaternion.Lerp(startRot, target.rotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = target.position;
        cam.rotation = target.rotation;
    }

    // Helper: Friends dance toward leader
    IEnumerator FriendDanceOver(Animator anim, Transform trans)
    {
        anim.Play(friendDanceClip.name);

        // Move toward leader while dancing
        Vector3 target = leaderTransform.position + new Vector3(
            Random.Range(-1f, 1f), 
            0,
            Random.Range(-1f, 1f)
        );

        yield return MoveTo(trans, target, 2f);
    }
}
