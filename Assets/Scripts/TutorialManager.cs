using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep {
        public Transform targetPosition;  // Where to point the arrow
        public string message;            // Toast message (optional)
        public string buttonName;         // Name of button to wait for (optional)
        public float rotation;            // Arrow rotation in degrees
        public bool bounceVertically = false; 
    }

    public bool isActive = true;

    public TutorialStep[] steps;
    public GameObject arrow;  // Your arrow GameObject
    
    public int currentStepIndex = 0;
    public static TutorialManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate TutorialManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

   void Start()
    {
        Debug.Log("TutorialManager Start() called!");
        Debug.Log($"Arrow is: {arrow}");
        Debug.Log($"Steps count: {steps.Length}");
        
        arrow.SetActive(true);
        isActive = true;
        StartNextStep();
    }

    void Update()
    {
        List<BuildingBase> farms = BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm);
        if(farms.Count > 0 && farms[0].GetMonkeyCount() >= 1)
        {
            if (currentStepIndex == 5 && isActive)
            {
                disableArrow();
                ToastManager.Instance.RequestToast("You can move monkeys between any two buildings this way.\n(Hint: you'll need to do this soon!)", 5f);
            }
        }
    }

    public void OnStepCompleted() {
        Debug.Log($"Completed step {currentStepIndex}: {steps[currentStepIndex].message}");
        
        StopAllCoroutines();
        currentStepIndex++;
        
        Debug.Log($"Moving to step {currentStepIndex}");
        
        // CHECK if there are more steps before accessing
        if (currentStepIndex == steps.Length - 1)
        {
            // 1. Get World Position of the farm
            Vector3 worldPos = BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm)[0].transform.position;
            
            // 2. Convert World Position -> Screen Space
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            // 3. Convert Screen Space -> UI Canvas Local Space
            // We need the arrow's parent to determine local coordinates correctly
            RectTransform canvasRect = arrow.transform.parent as RectTransform;
            Vector2 localPos;
            
            // Note: Pass 'null' for the camera if your Canvas Render Mode is "Screen Space - Overlay".
            // If it is "Screen Space - Camera", pass 'Camera.main'.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                screenPos, 
                null, 
                out localPos
            );

            localPos.x += 80;
            // 4. Pass the calculated Vector2 to the helper function
            StartNextStep(localPos);
        } else if (currentStepIndex < steps.Length) {
            Debug.Log($"Next step target: {steps[currentStepIndex].targetPosition.name}");
            StartNextStep();
        } else {
            Debug.Log("No more steps, tutorial complete");
            arrow.SetActive(false);
            isActive = false;
        }
    }

    // Change parameter to Vector2? to accept coordinate overrides
    void StartNextStep(Vector2? overridePos = null) {

        TutorialStep step = steps[currentStepIndex];
        
        Debug.Log($"Starting step {currentStepIndex}: Message = {step.message}");
        
        RectTransform arrowRect = arrow.GetComponent<RectTransform>();
        Vector2 targetPos;

        // LOGIC: If we provided a manual position (from the 3D farm), use it.
        // Otherwise, get the position from the UI target in the step array.
        if (overridePos.HasValue)
        {
            targetPos = overridePos.Value;
            // Optional: Reset rotation for 3D objects if needed, or keep previous
            arrow.transform.rotation = Quaternion.identity; 
        }
        else
        {
            RectTransform targetRect = step.targetPosition.GetComponent<RectTransform>();
            targetPos = targetRect.anchoredPosition;
            arrow.transform.rotation = step.targetPosition.rotation;
        }
        
        arrowRect.anchoredPosition = targetPos;
        
        // Pass the direction to the coroutine
        StartCoroutine(BounceArrow(targetPos, step.bounceVertically));
        
        if (!string.IsNullOrEmpty(step.message)) {
            ToastManager.Instance.ReplaceToast(step.message, 3f);
        }
    }

    IEnumerator BounceArrow(Vector2 originalPos, bool vertical) {
        RectTransform arrowRect = arrow.GetComponent<RectTransform>();
        
        while (true) {
            float offset = Mathf.Sin(Time.time * 2f) * 15f;
            
            if (vertical) {
                arrowRect.anchoredPosition = originalPos + new Vector2(0, offset);  // Bounce up/down
            } else {
                arrowRect.anchoredPosition = originalPos + new Vector2(offset, 0);  // Bounce left/right
            }
            
            yield return null;
        }
    }

    public void disableArrow() {
        arrow.SetActive(false);
        isActive = false;
    }
}