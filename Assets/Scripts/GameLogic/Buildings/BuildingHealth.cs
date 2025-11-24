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
    private BuildingBase building = null;

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

    // now returns the amount of damage the chimp should take
    public float TakeDamage(float damage)
    {
        if (currentHealth <= 0) return 0; 

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
        if(building.GetBuildingType() == BuildingType.SpikeTrap) return (building as SpikeTrap).damageMultiplier * damage;
        return 0;
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
        // Call the BuildingBase's OnDestroy method to properly free monkeys
        BuildingBase buildingBase = GetComponent<BuildingBase>();
        if (buildingBase != null)
        {
            Debug.Log($"[BuildingHealth] Calling OnDestroy for {buildingBase.GetType().Name} to free monkeys");
            buildingBase.OnDestroy();
        }
        
        // Now destroy the building
        Destroy(gameObject);
    }

    public void SetBuilding(BuildingBase b) => building = b;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public void SetMaxHealth(float h) => maxHealth = h;
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