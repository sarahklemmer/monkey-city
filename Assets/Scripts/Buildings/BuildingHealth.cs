using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Linq;

public class BuildingHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float regenDelay = 60f; 
    [SerializeField] private float regenRate = 5f; 
    
    private float currentHealth;
    private float lastDamageTime;
    private bool isRegenerating = false;
    private Coroutine regenCoroutine;

    [SerializeField] private Renderer buildingRenderer;
    [SerializeField] private Color fullHealthColor = Color.white;
    [SerializeField] private Color lowHealthColor = Color.red;

    void Awake()
    {
        currentHealth = maxHealth;
        lastDamageTime = -regenDelay;
        Assert.IsNotNull(buildingRenderer, "you forgot to assign a buildingrenderer in the inspector!");
    }

    void Start()
    {
        UpdateVisuals();
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return; 

        currentHealth = Mathf.Max(0, currentHealth - damage);
        lastDamageTime = Time.time;
        
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            isRegenerating = false;
        }

        UpdateVisuals();

        if (currentHealth <= 0)
        {
            OnDestroyed();
        }
        else
        {
            regenCoroutine = StartCoroutine(CheckForRegeneration());
        }
    }

    private IEnumerator CheckForRegeneration()
    {
        while (Time.time - lastDamageTime < regenDelay)
        {
            yield return new WaitForSeconds(0.5f);
        }

        isRegenerating = true;
        
        while (currentHealth < maxHealth && isRegenerating)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + regenRate * Time.deltaTime);

            UpdateVisuals();
            yield return null;
        }

        isRegenerating = false;
        regenCoroutine = null;
    }

    private void UpdateVisuals()
    {
        float healthPercent = currentHealth / maxHealth;
        Color targetColor = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        //the last one is the glow material
        foreach (Material mat in buildingRenderer.materials.SkipLast(1))
        {
            if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", targetColor);
            }
            
            mat.SetColor("_Color", targetColor);
        }
    }

    private void OnDestroyed()
    {
        // TODO: Handle building destruction
        
        Destroy(gameObject);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercent() => currentHealth / maxHealth;
    public bool IsRegenerating() => isRegenerating;
    public float GetTimeSinceLastDamage() => Time.time - lastDamageTime;
    public float GetTimeUntilRegen() => Mathf.Max(0, regenDelay - GetTimeSinceLastDamage());

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateVisuals();
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
        UpdateVisuals();
        
        // Stop any ongoing regeneration
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            isRegenerating = false;
            regenCoroutine = null;
        }
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        UpdateVisuals();
    }
}