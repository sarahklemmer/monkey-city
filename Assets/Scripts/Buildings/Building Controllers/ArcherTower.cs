using System;
using UnityEngine;

public class ArcherTower : BuildingBase
{
    private int level = 1;
    private const int MAX_LEVEL = 3;
    
    private float attackRange = 5f;
    private float attackDamage = 10f;
    private float attackCooldown = 1f; 
    
    [SerializeField] GameObject arrowPrefab;
    
    // Visual models - these should be child GameObjects in your hierarchy
    [SerializeField] GameObject level1Model; // Assign the initial tower model
    [SerializeField] GameObject level2Model; // Assign the level 2 visual child object
    [SerializeField] GameObject level3Model; // Assign the level 3 visual child object
    
    [SerializeField] ParticleSystem upgradeEffect; // Assign sparkle particle system
    
    private Transform firePoint;
    private float lastAttackTime;
    private EnemyAttacker targetEnemy;

    // Range indicator components
    private LineRenderer rangeIndicator;
    [SerializeField] private bool showRangeOnSelect = true;
    [SerializeField] private Color rangeColor = new Color(0.5f, 0.8f, 1f, 0.15f);
    [SerializeField] private int circleSegments = 50;
    [SerializeField] private Vector3 circleOffset = Vector3.zero;

    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.ArcherTower);
        monkeys = new(1); // Capacity of 1 monkey
        
        // Make sure upgrade models start disabled
        if (level1Model != null)
        {
            level1Model.SetActive(true);
            Debug.Log("[ArcherTower] Level 1 model found and enabled");
        }
        else
        {
            Debug.LogWarning("[ArcherTower] Level 1 model not assigned!");
        }
        
        if (level2Model != null)
        {
            level2Model.SetActive(false);
            Debug.Log("[ArcherTower] Level 2 model found and disabled");
        }
        else
        {
            Debug.LogWarning("[ArcherTower] Level 2 model not assigned!");
        }
        
        if (level3Model != null)
        {
            level3Model.SetActive(false);
            Debug.Log("[ArcherTower] Level 3 model found and disabled");
        }
        
        CreateRangeIndicator();
    }

    void Start()
    {
        // ALWAYS show range indicator for testing
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = true;
            Debug.Log("[ArcherTower] Range indicator enabled!");
        }
        else
        {
            Debug.LogError("[ArcherTower] Range indicator is NULL!");
        }
    }

    private void CreateRangeIndicator()
    {
        Debug.Log("[ArcherTower] Creating range indicator...");
        
        GameObject rangeObj = new GameObject("RangeIndicator");
        rangeObj.transform.SetParent(transform);
        rangeObj.transform.localPosition = circleOffset;
        
        rangeIndicator = rangeObj.AddComponent<LineRenderer>();
        rangeIndicator.useWorldSpace = false;
        rangeIndicator.loop = true;
        rangeIndicator.positionCount = circleSegments;
        rangeIndicator.startWidth = 0.08f;
        rangeIndicator.endWidth = 0.08f;
        
        Material mat = new Material(Shader.Find("Sprites/Default"));
        if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
        {
            Debug.LogWarning("[ArcherTower] Sprites/Default not found, trying Unlit/Color");
            mat = new Material(Shader.Find("Unlit/Color"));
        }
        if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
        {
            Debug.LogWarning("[ArcherTower] Unlit/Color not found, trying Particles/Standard Unlit");
            mat = new Material(Shader.Find("Particles/Standard Unlit"));
        }
        
        rangeIndicator.material = mat;
        rangeIndicator.startColor = rangeColor;
        rangeIndicator.endColor = rangeColor;
        
        rangeIndicator.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rangeIndicator.receiveShadows = false;
        
        UpdateRangeCircle();
        
        Debug.Log($"[ArcherTower] Range indicator created with {circleSegments} segments, range: {attackRange}");
    }

    private void UpdateRangeCircle()
    {
        if (rangeIndicator == null)
        {
            Debug.LogError("[ArcherTower] Cannot update range circle - rangeIndicator is null!");
            return;
        }
        
        float angleStep = 360f / circleSegments;
        
        for (int i = 0; i < circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * attackRange;
            float z = Mathf.Sin(angle) * attackRange;
            
            Vector3 position = new Vector3(x, 0.1f, z);
            
            rangeIndicator.SetPosition(i, position);
        }
        
        Debug.Log($"[ArcherTower] Range circle updated: {circleSegments} points at range {attackRange}");
    }

    private bool HasMonkey()
    {
        return monkeys != null && monkeys.Count() > 0;
    }

    void Update()
    {
        // 5 ^ (level - 1) so -1, -5, -25 times number of monkeys + 1 so it still costs bananas to defend
        bananasPerDay = ((int)Math.Pow(5, level - 1)) * GetMonkeyCount() * -1;

        attackDamage = AllArcherTowerInfo.instance.GetDamagePerAttack();

        if (!HasMonkey())
        {
            if (targetEnemy != null)
            {
                targetEnemy = null;
            }
            return;
        }

        if (targetEnemy == null)
        {
            FindNearestEnemy();
            return;
        }

        if (!IsInRange(targetEnemy) || targetEnemy.GetCurrentHealth() <= 0)
        {
            targetEnemy = null;
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    public void OnSelected()
    {
        if (rangeIndicator != null && showRangeOnSelect)
        {
            rangeIndicator.enabled = true;
        }
    }

    public void OnDeselected()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = false;
        }
    }

    public void SetRangeIndicatorVisible(bool visible)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = visible;
        }
    }

    private void FindNearestEnemy()
    {
        EnemyAttacker[] enemies = FindObjectsByType<EnemyAttacker>(FindObjectsSortMode.None);
        
        float closestDist = Mathf.Infinity;
        EnemyAttacker closest = null;
        
        foreach (var enemy in enemies)
        {
            if (enemy.GetCurrentHealth() <= 0) continue;
            
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            
            if (dist <= attackRange && dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        targetEnemy = closest;
    }

    private bool IsInRange(EnemyAttacker enemy)
    {
        if (enemy == null) return false;
        float dist = Vector3.Distance(transform.position, enemy.transform.position);
        return dist <= attackRange;
    }

    private void Attack()
    {
        if (targetEnemy == null) return;
        if (arrowPrefab != null)
        {
            SpawnArrow();
        }
    }

    private void SpawnArrow()
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up;
        Debug.Log($"[ArcherTower] Spawning arrow at {spawnPos}");
        
        if (arrowPrefab == null)
        {
            Debug.LogError("[ArcherTower] Arrow prefab is NULL! Assign it in the Inspector!");
            return;
        }
        
        GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"[ArcherTower] Arrow instantiated: {arrow.name}");
        
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null && targetEnemy != null)
        {
            arrowScript.Initialize(targetEnemy, attackDamage);
            Debug.Log($"[ArcherTower] Arrow initialized with target: {targetEnemy.name}, damage: {attackDamage}");
        }
        else
        {
            Debug.LogError($"[ArcherTower] Arrow script: {arrowScript}, Target: {targetEnemy}");
        }
    }

    public void Upgrade()
    {
        if (level >= MAX_LEVEL) return;
        
        level++;
        
        switch (level)
        {
            case 2:
                // Level 2: Faster shooting only
                attackCooldown = 0.7f;
                
                // Hide level 1 model and show level 2 model
                if (level1Model != null)
                {
                    Debug.Log($"[ArcherTower] Deactivating Level 1 model: {level1Model.name}, was active: {level1Model.activeSelf}");
                    level1Model.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("[ArcherTower] Level 1 model is NULL!");
                }
                
                if (level2Model != null)
                {
                    Debug.Log($"[ArcherTower] Activating Level 2 model: {level2Model.name}, was active: {level2Model.activeSelf}");
                    level2Model.SetActive(true);
                    Debug.Log($"[ArcherTower] Level 2 model is now active: {level2Model.activeSelf}");
                    
                    // Check if it has a renderer
                    Renderer renderer = level2Model.GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        Debug.Log($"[ArcherTower] Level 2 model has renderer, enabled: {renderer.enabled}");
                    }
                    else
                    {
                        Debug.LogError("[ArcherTower] Level 2 model has NO RENDERER!");
                    }
                }
                else
                {
                    Debug.LogWarning("[ArcherTower] Level 2 model is NULL! Did you assign it in the Inspector?");
                }
                break;
            case 3:
                // Level 3: Increased range and even faster shooting
                attackRange = 6.5f;
                attackCooldown = 0.5f;
                
                // Hide level 2 model and show level 3 model
                if (level2Model != null)
                {
                    level2Model.SetActive(false);
                }
                
                if (level3Model != null)
                {
                    level3Model.SetActive(true);
                    Debug.Log("[ArcherTower] Activating Level 3 model");
                }
                break;
        }
        
        // Play sparkle upgrade effect
        if (upgradeEffect != null)
        {
            upgradeEffect.Play();
        }
        
        // Update the range circle when upgrading
        UpdateRangeCircle();
        
        Debug.Log($"Archer Tower upgraded to level {level}!");
    }

    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    public float GetAttackRange() => attackRange;
    //public float GetAttackDamage() => attackDamage;
    public float GetAttackCooldown() => attackCooldown;
    
    public int GetUpgradeCost()
    {
        if (level >= MAX_LEVEL) return 0;
        
        switch (level)
        {
            case 1: return 50;  // Level 1 -> 2 costs 50
            case 2: return 120; // Level 2 -> 3 costs 120
            default: return 0;
        }
    }

    public override void OnDayCycle()
    {
        /* do nothing */
    }

    public override void OnDestroy()
    {
        /* do nothing */
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (targetEnemy != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetEnemy.transform.position);
        }
    }
}