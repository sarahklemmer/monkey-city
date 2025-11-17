using UnityEngine;
using TMPro;

public class BananaVisualization : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private SpriteRenderer bananaIcon;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float fadeStartTime = 1f;
    
    private float timer = 0f;
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        
        // Auto-find TextMeshPro if not assigned
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshPro>();
    }
    
    public void Initialize(int amount)
    {
        if (textMesh == null) 
            textMesh = GetComponentInChildren<TextMeshPro>();
        
        if (amount > 0)
        {
            textMesh.text = $"+{amount}";
            textMesh.color = Color.yellow;
        }
        else
        {
            textMesh.text = $"{amount}";
            textMesh.color = Color.red;
        }
    }
    
    public void Initialize(string text, Color color)
    {
        if (textMesh == null) 
            textMesh = GetComponentInChildren<TextMeshPro>();
        
        textMesh.text = text;
        textMesh.color = color;
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        // ALWAYS face the camera - this is the important part!
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0); // Flip so text isn't backwards
        }
        
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        
        // Fade out near the end
        if (timer > fadeStartTime)
        {
            float alpha = 1f - ((timer - fadeStartTime) / (lifetime - fadeStartTime));
            
            // Fade text
            Color c = textMesh.color;
            c.a = alpha;
            textMesh.color = c;
            
            if (bananaIcon != null)
            {
                Color iconColor = bananaIcon.color;
                iconColor.a = alpha;
                bananaIcon.color = iconColor;
            }
        }
        
        // Destroy after lifetime
        if (timer > lifetime)
        {
            Destroy(gameObject);
        }
    }
}