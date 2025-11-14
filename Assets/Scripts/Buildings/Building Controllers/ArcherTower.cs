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
    private Transform firePoint;
    private float lastAttackTime;
    private EnemyAttacker targetEnemy;

    // Range indicator components
    private LineRenderer rangeIndicator;
    [SerializeField] private bool showRangeOnSelect = true;
    [SerializeField] private Color rangeColor = new Color(0.5f, 0.8f, 1f, 0.15f); // Subtle blue with low transparency
    [SerializeField] private int circleSegments = 50;
    [SerializeField] private Vector3 circleOffset = Vector3.zero; // Adjust if tower isn't centered

    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.ArcherTower);
        monkeys = new(1);
        
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
        
        // Create a new GameObject for the range indicator
        GameObject rangeObj = new GameObject("RangeIndicator");
        rangeObj.transform.SetParent(transform);
        rangeObj.transform.localPosition = circleOffset; // Apply offset
        
        // Add and configure LineRenderer
        rangeIndicator = rangeObj.AddComponent<LineRenderer>();
        rangeIndicator.useWorldSpace = false;
        rangeIndicator.loop = true;
        rangeIndicator.positionCount = circleSegments;
        rangeIndicator.startWidth = 0.08f;  // Thinner for subtlety
        rangeIndicator.endWidth = 0.08f;
        
        // Try multiple shaders to find one that works
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
        
        // Disable shadows
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
            
            // Try 3D first (flat on ground)
            Vector3 position = new Vector3(x, 0.1f, z); // Slightly elevated
            
            // If you have a 2D game, uncomment this instead:
            // Vector3 position = new Vector3(x, z, 0);
            
            rangeIndicator.SetPosition(i, position);
        }
        
        Debug.Log($"[ArcherTower] Range circle updated: {circleSegments} points at range {attackRange}");
    }

    void Update()
    {
        // 5 ^ (level - 1) so -1, -5, -25 times number of monkeys + 1 so it still costs bananas to defend
        bananasPerDay = ((int)Math.Pow(5, level - 1)) * GetMonkeyCount() * -1;

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

    // Call this method when the tower is selected/clicked
    public void OnSelected()
    {
        if (rangeIndicator != null && showRangeOnSelect)
        {
            rangeIndicator.enabled = true;
        }
    }

    // Call this method when the tower is deselected
    public void OnDeselected()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = false;
        }
    }

    // Optional: Always show range indicator
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
                attackRange = 7f;
                attackDamage = 15f;
                attackCooldown = 0.8f;
                break;
            case 3:
                attackRange = 10f;
                attackDamage = 25f;
                attackCooldown = 0.6f;
                break;
        }
        
        // Update the range circle when upgrading
        UpdateRangeCircle();
        
        transform.GetChild(0).gameObject.SetActive(true);
        Debug.Log($"Archer Tower upgraded to level {level}!");
    }

    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    public float GetAttackRange() => attackRange;
    public float GetAttackDamage() => attackDamage;
    public float GetAttackCooldown() => attackCooldown;

    public override void OnDayCycle()
    {
        // do nothing
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