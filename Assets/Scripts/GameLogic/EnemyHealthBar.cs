using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAttacker enemy;
    
    [Header("Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
    [SerializeField] private Vector2 size = new Vector2(0.5f, 0.08f);
    [SerializeField] private Color healthColor = Color.green;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private bool alwaysFaceCamera = true;

    private Camera mainCamera;
    private GameObject canvasObj;
    private Image backgroundImage;
    private Image fillImage;

    void Start()
    {
        mainCamera = Camera.main;
        
        // Auto-find enemy if not assigned
        if (enemy == null)
        {
            enemy = GetComponentInParent<EnemyAttacker>();
            if (enemy == null)
            {
                Debug.LogError("[EnemyHealthBar] Could not find EnemyAttacker component!");
                return;
            }
        }
        
        CreateHealthBar();
        UpdateHealthBar();
    }

    void LateUpdate()
    {
        if (canvasObj == null) return;
        
        if (alwaysFaceCamera && mainCamera != null)
        {
            canvasObj.transform.LookAt(canvasObj.transform.position + mainCamera.transform.rotation * Vector3.forward,
                                      mainCamera.transform.rotation * Vector3.up);
        }
        
        UpdateHealthBar();
    }

    private void CreateHealthBar()
    {
        // Create canvas GameObject
        canvasObj = new GameObject("HealthBarCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = offset;
        canvasObj.transform.localRotation = Quaternion.identity;
        canvasObj.transform.localScale = Vector3.one;
        
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 100;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = size * 50f;
        
        // Create background (red - shows damage)
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        
        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = damageColor;
        
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = Vector2.zero;
        
        // Create foreground (green - shows health)
        GameObject fgObj = new GameObject("Foreground");
        fgObj.transform.SetParent(canvasObj.transform, false);
        
        fillImage = fgObj.AddComponent<Image>();
        fillImage.color = healthColor;
        
        RectTransform fgRect = fgObj.GetComponent<RectTransform>();
        fgRect.anchorMin = new Vector2(0, 0);
        fgRect.anchorMax = new Vector2(1, 1);
        fgRect.pivot = new Vector2(0, 0.5f);
        fgRect.anchoredPosition = Vector2.zero;
        fgRect.sizeDelta = Vector2.zero;
        
        Debug.Log("[EnemyHealthBar] Health bar created successfully");
    }

    private void UpdateHealthBar()
    {
        if (enemy == null || fillImage == null || canvasObj == null) return;
        
        float healthPercent = enemy.GetHealthPercentage();
        
        // Scale the foreground horizontally to show health
        RectTransform fgRect = fillImage.GetComponent<RectTransform>();
        fgRect.anchorMax = new Vector2(healthPercent, 1);
        
        // Hide when at full health if enabled
        if (hideWhenFull)
        {
            canvasObj.SetActive(healthPercent < 1f);
        }
    }

    public void ForceUpdate()
    {
        UpdateHealthBar();
    }
}