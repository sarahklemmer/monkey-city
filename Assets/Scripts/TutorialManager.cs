using UnityEngine;
using System.Collections;
using UnityEngine.UI;  

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

   void Start()
    {
        Debug.Log("TutorialManager Start() called!");
        Debug.Log($"Arrow is: {arrow}");
        Debug.Log($"Steps count: {steps.Length}");
        
        arrow.SetActive(true);
        isActive = true;
        StartNextStep();
    }

    public void OnStepCompleted() {
        Debug.Log($"Completed step {currentStepIndex}: {steps[currentStepIndex].message}");
        
        StopAllCoroutines();
        currentStepIndex++;
        
        Debug.Log($"Moving to step {currentStepIndex}");
        
        // CHECK if there are more steps before accessing
        if (currentStepIndex < steps.Length) {
            Debug.Log($"Next step target: {steps[currentStepIndex].targetPosition.name}");
            StartNextStep();
        } else {
            Debug.Log("No more steps, tutorial complete");
            arrow.SetActive(false);
            isActive = false;
        }
    }

    void StartNextStep() {

        TutorialStep step = steps[currentStepIndex];
        
        Debug.Log($"Starting step {currentStepIndex}: Target = {step.targetPosition.name}, Message = {step.message}");
        
        RectTransform arrowRect = arrow.GetComponent<RectTransform>();
        RectTransform targetRect = step.targetPosition.GetComponent<RectTransform>();
        
        arrowRect.anchoredPosition = targetRect.anchoredPosition;
        arrow.transform.rotation = step.targetPosition.rotation;
        
        // Pass the direction to the coroutine
        StartCoroutine(BounceArrow(targetRect.anchoredPosition, step.bounceVertically));
        
        if (!string.IsNullOrEmpty(step.message)) {
            ToastManager.Instance.RequestToast(step.message, 3f);
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