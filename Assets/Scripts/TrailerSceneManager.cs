using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonkeyCutscene : MonoBehaviour
{
    [Header("Leader Monkey")]
    public Transform leaderTransform;

    [Header("Friends")]
    public List<Transform> friendTransforms;

    [Header("Scene Targets")]
    public Transform treeTransform;

    [Header("Camera")]
    public Camera cutsceneCamera;
    public Transform camStart;
    public Transform camWideShot;
    public Transform camTreeFocus;

    [Header("Settings")]
    public float moveSpeed = 8f;
    public float jumpHeight = 2f;

    void Start()
    {
        StartCoroutine(RunCutscene());
    }

    IEnumerator RunCutscene()
    {
        Debug.Log("Scene 1: Setting up camera");
        
        // --- SCENE 1: Camera pans across the jungle ---
        if (cutsceneCamera == null || camStart == null)
        {
            Debug.LogError("Camera or CamStart is NULL!");
            yield break;
        }
        
        cutsceneCamera.transform.position = camStart.position;
        cutsceneCamera.transform.rotation = camStart.rotation;
        Debug.Log("Camera moved to start position");
        yield return new WaitForSeconds(0.5f);

        // Pan to wide shot
        Debug.Log("Scene 1: Panning to wide shot");
        if (camWideShot == null)
        {
            Debug.LogError("CamWideShot is NULL!");
            yield break;
        }
        yield return MoveCamera(cutsceneCamera.transform, camWideShot, 1.5f);
        Debug.Log("Camera panned to wide shot");
        yield return new WaitForSeconds(0.3f);

        // --- SCENE 2: Leader spots the tree and gets excited ---
        // Leader jumps up and down (excited!)
        for (int i = 0; i < 2; i++)
        {
            yield return Jump(leaderTransform, jumpHeight * 0.7f, 0.25f);
            yield return new WaitForSeconds(0.05f);
        }

        // Leader runs toward tree
        Vector3 treeSpot = treeTransform.position + new Vector3(0, 0, -3f);
        yield return MoveToWithBounce(leaderTransform, treeSpot, moveSpeed * 2f);
        
        // Leader looks up at tree
        leaderTransform.LookAt(treeTransform.position + Vector3.up * 5f);
        yield return new WaitForSeconds(0.2f);

        // Leader jumps and waves arms (calling friends)
        yield return Jump(leaderTransform, jumpHeight, 0.3f);

        // --- SCENE 3: Friends notice and get excited ---
        yield return new WaitForSeconds(0.2f);
        
        // All friends jump excited
        Debug.Log($"Making {friendTransforms.Count} friends jump!");
        foreach (var friend in friendTransforms)
        {
            if (friend != null)
            {
                Debug.Log($"Friend {friend.name} is jumping!");
                StartCoroutine(Jump(friend, jumpHeight * 0.5f, 0.25f));
            }
            else
            {
                Debug.LogError("Found a NULL friend in the list!");
            }
        }
        yield return new WaitForSeconds(0.3f);

        // --- SCENE 4: Everyone rushes to the tree ---
        // Friends run over with slight delays
        Debug.Log("Friends starting to run to tree!");
        List<Coroutine> friendMovements = new List<Coroutine>();
        for (int i = 0; i < friendTransforms.Count; i++)
        {
            if (friendTransforms[i] != null)
            {
                Vector3 friendSpot = treeTransform.position + new Vector3(
                    Random.Range(-2f, 2f), 
                    0, 
                    Random.Range(-3f, -1f)
                );
                Debug.Log($"Friend {friendTransforms[i].name} moving to {friendSpot}");
                StartCoroutine(MoveToWithBounce(friendTransforms[i], friendSpot, moveSpeed * 1.3f));
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Wait for all friends to arrive
        yield return new WaitForSeconds(2f);
        Debug.Log("All friends should be at tree now");

        // --- SCENE 5: Camera zooms to tree ---
        yield return MoveCamera(cutsceneCamera.transform, camTreeFocus, 1.5f);

        // --- SCENE 6: Everyone celebrates around the tree ---
        Debug.Log("Starting celebration!");
        // Everyone jumps together
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(Jump(leaderTransform, jumpHeight, 0.3f));
            foreach (var friend in friendTransforms)
            {
                Debug.Log($"Friend {friend.name} jumping!");
                StartCoroutine(Jump(friend, jumpHeight * Random.Range(0.7f, 1f), 0.3f));
            }
            yield return new WaitForSeconds(0.4f);
        }

        // --- SCENE 7: Everyone climbs up the tree ---
        Vector3 treeTop = treeTransform.position + Vector3.up * 5f;
        
        // Leader goes first
        yield return MoveToSmoothly(leaderTransform, treeTop, 1.5f);
        
        // Friends follow
        for (int i = 0; i < friendTransforms.Count; i++)
        {
            Vector3 friendTarget = treeTop + new Vector3(
                Random.Range(-0.5f, 0.5f), 
                Random.Range(-0.5f, 0.5f), 
                Random.Range(-0.5f, 0.5f)
            );
            StartCoroutine(MoveToSmoothly(friendTransforms[i], friendTarget, 1.5f));
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1.5f);

        // Final celebration jump at the top
        StartCoroutine(Jump(leaderTransform, jumpHeight * 0.5f, 0.25f));
        foreach (var friend in friendTransforms)
        {
            StartCoroutine(Jump(friend, jumpHeight * 0.4f, 0.25f));
        }

        yield return new WaitForSeconds(0.8f);
        Debug.Log("🐵 Cutscene Complete! 🌳");
    }

    // Helper: Jump in place
    IEnumerator Jump(Transform obj, float height, float duration)
    {
        Vector3 startPos = obj.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Parabolic jump arc
            float yOffset = Mathf.Sin(t * Mathf.PI) * height;
            obj.position = startPos + Vector3.up * yOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = startPos;
    }

    // Helper: Move with bouncy walk
    IEnumerator MoveToWithBounce(Transform obj, Vector3 target, float speed)
    {
        if (obj == null) yield break;
        
        Vector3 startPos = obj.position;
        float baseY = startPos.y;

        // Face target
        Vector3 direction = (target - obj.position).normalized;
        direction.y = 0; // Keep on same Y level
        if (direction != Vector3.zero)
            obj.rotation = Quaternion.LookRotation(direction);

        float maxTime = 10f; // Safety timeout
        float elapsed = 0f;
        
        while (Vector3.Distance(new Vector3(obj.position.x, 0, obj.position.z), new Vector3(target.x, 0, target.z)) > 0.1f && elapsed < maxTime)
        {
            // Move forward
            Vector3 newPos = Vector3.MoveTowards(obj.position, target, speed * Time.deltaTime);
            
            // Add bounce while walking
            float bounce = Mathf.Abs(Mathf.Sin(Time.time * 10f)) * 0.2f;
            newPos.y = baseY + bounce;
            
            obj.position = newPos;
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = new Vector3(target.x, baseY, target.z);
    }

    // Helper: Smooth movement (for climbing)
    IEnumerator MoveToSmoothly(Transform obj, Vector3 target, float duration)
    {
        Vector3 startPos = obj.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Ease in-out
            t = t * t * (3f - 2f * t);
            obj.position = Vector3.Lerp(startPos, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.position = target;
    }

    // Helper: Camera movement
    IEnumerator MoveCamera(Transform cam, Transform target, float duration)
    {
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Smooth camera movement
            t = t * t * (3f - 2f * t);
            cam.position = Vector3.Lerp(startPos, target.position, t);
            cam.rotation = Quaternion.Lerp(startRot, target.rotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = target.position;
        cam.rotation = target.rotation;
    }
}