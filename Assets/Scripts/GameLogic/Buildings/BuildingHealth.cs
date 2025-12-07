using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class BuildingHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float regenDelay = 60f; 
    [SerializeField] private float regenRate = 5f; 
    
    private float currentHealth;
    private float lastDamageTime;
    private bool isRegenerating = false;
    [HideInInspector] public bool regenerationAllowed = true;
    private Coroutine regenCoroutine;
    private BuildingBase building = null;

    [SerializeField] private Renderer buildingRenderer;
    
    [Header("Damage Tint Settings")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] [Range(0f, 1f)] private float maxDamageTint = 0.4f;
    [SerializeField] [Range(1f, 5f)] private float damageSeverity = 1.5f;
    
    private Dictionary<int, Color> originalColors = new Dictionary<int, Color>();

    void Awake()
    {
        currentHealth = maxHealth;
        lastDamageTime = -regenDelay;
        Assert.IsNotNull(buildingRenderer, "you forgot to assign a buildingrenderer in the inspector!");
    }

    void Start()
    {
        StoreOriginalColors();
        UpdateVisuals();
    }

    private void StoreOriginalColors()
    {
        if (buildingRenderer == null) return;
        
        Material[] materials = buildingRenderer.materials;
        
        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            
            if (mat.name.Contains("Outline") || mat.name.Contains("Glow"))
            {
                continue;
            }
            
            Color originalColor = Color.white;
            
            if (mat.HasProperty("_BaseColor"))
            {
                originalColor = mat.GetColor("_BaseColor");
            }
            else if (mat.HasProperty("_Color"))
            {
                originalColor = mat.GetColor("_Color");
            }
            
            originalColors[i] = originalColor;
        }
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
        while (Time.time - lastDamageTime < regenDelay || !regenerationAllowed)
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
        if (buildingRenderer == null) return;
        
        float healthPercent = currentHealth / maxHealth;
        float damageAmount = 1f - healthPercent;
        float adjustedDamage = Mathf.Pow(damageAmount, 1f / damageSeverity);
        float tintAmount = adjustedDamage * maxDamageTint;
        
        Material[] materials = buildingRenderer.materials;
        
        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            
            if (mat.name.Contains("Outline") || mat.name.Contains("Glow") || mat.name.Contains("treecolor"))
            {
                continue;
            }
            
            if (!originalColors.ContainsKey(i))
            {
                continue;
            }
            
            Color originalColor = originalColors[i];
            Color targetColor = Color.Lerp(originalColor, damageColor, tintAmount);
            
            if (mat.HasProperty("_BaseColor"))
            {
                mat.SetColor("_BaseColor", targetColor);
            }
            else if (mat.HasProperty("_Color"))
            {
                mat.SetColor("_Color", targetColor);
            }
        }
    }

    private void OnDestroyed()
    {
        Debug.Log($"BuildingHealth: Building destroyed at health 0");
        
        BuildingBase buildingBase = GetComponent<BuildingBase>();
        if (buildingBase != null)
        {
            // Store grid position before calling Die()
            int gridX = buildingBase.grid_x;
            int gridY = buildingBase.grid_y;
            
            Debug.Log($"BuildingHealth: Calling Die() on {buildingBase.type} at grid ({gridX}, {gridY})");
            
            // Call Die() which handles all the cleanup including:
            // - Freeing monkeys (if applicable)
            // - Notifying BuildingUnlock
            // - Notifying BuildingManager
            // - Restoring placement indicator via BuildingGrid
            buildingBase.Die();
            
            // Die() will destroy the GameObject, so don't call Destroy here
        }
        else
        {
            Debug.LogError("BuildingHealth: No BuildingBase component found!");
            Destroy(gameObject);
        }
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