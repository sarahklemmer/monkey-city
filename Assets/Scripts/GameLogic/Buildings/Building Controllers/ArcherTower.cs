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
    [SerializeField] GameObject level1Model;
    [SerializeField] GameObject level2Model;
    [SerializeField] GameObject level3Model;
    
    [SerializeField] ParticleSystem upgradeEffect;
    
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
        monkeys = new(1);
    
        // Find and stop any looping particle systems
        ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in allParticles)
        {
            if (ps != upgradeEffect && ps.main.loop)
            {
                ps.Stop();
                ps.Clear();
            }
        }
        
        // Make sure upgrade models start disabled
        if (level1Model != null) level1Model.SetActive(true);
        if (level2Model != null) level2Model.SetActive(false);
        if (level3Model != null) level3Model.SetActive(false);
        
        CreateRangeIndicator();
    }

    void Start()
    {
        if (BuildingSoundManager.instance != null)
        {
            BuildingSoundManager.instance.PlayBuildingPlacedSound();
        }
    
        if (WaveSpawner.instance != null)
        {
            WaveSpawner.instance.OnFirstTowerPlaced();
        }
    
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = true;
        }
    }

    private void CreateRangeIndicator()
    {
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
            mat = new Material(Shader.Find("Unlit/Color"));
        }
        if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
        {
            mat = new Material(Shader.Find("Particles/Standard Unlit"));
        }
        
        rangeIndicator.material = mat;
        rangeIndicator.startColor = rangeColor;
        rangeIndicator.endColor = rangeColor;
        rangeIndicator.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rangeIndicator.receiveShadows = false;
        
        UpdateRangeCircle();
    }

    private void UpdateRangeCircle()
    {
        if (rangeIndicator == null) return;
        
        float angleStep = 360f / circleSegments;
        
        for (int i = 0; i < circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * attackRange;
            float z = Mathf.Sin(angle) * attackRange;
            Vector3 position = new Vector3(x, 0.1f, z);
            rangeIndicator.SetPosition(i, position);
        }
    }

    private bool HasMonkey()
    {
        return monkeys != null && monkeys.Count() > 0;
    }

    void Update()
    {
        bananasPerDay = ((int)Math.Pow(5, level - 1)) * GetMonkeyCount() * -1;
        attackDamage = AllArcherTowerInfo.instance.GetDamagePerAttack();

        if (!HasMonkey())
        {
            if (targetEnemy != null) targetEnemy = null;
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
        
        if (arrowPrefab == null)
        {
            Debug.LogError("[ArcherTower] Arrow prefab is NULL!");
            return;
        }
        
        GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        
        if (arrowScript != null && targetEnemy != null)
        {
            arrowScript.Initialize(targetEnemy, attackDamage);
        }
    }

    public void Upgrade()
    {
        if (level >= MAX_LEVEL) return;
        
        level++;
        
        // Play upgrade sound
        if (BuildingSoundManager.instance != null)
        {
            BuildingSoundManager.instance.PlayUpgradeSound();
        }
        
        switch (level)
        {
            case 2:
                attackCooldown = 0.7f;
                if (level1Model != null) level1Model.SetActive(false);
                if (level2Model != null) level2Model.SetActive(true);
                break;
            case 3:
                attackRange = 6.5f;
                attackCooldown = 0.5f;
                if (level2Model != null) level2Model.SetActive(false);
                if (level3Model != null) level3Model.SetActive(true);
                break;
        }
        
        if (upgradeEffect != null)
        {
            upgradeEffect.Play();
        }
        
        UpdateRangeCircle();
        
        Debug.Log($"Archer Tower upgraded to level {level}!");
    }

    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    public float GetAttackRange() => attackRange;
    public float GetAttackCooldown() => attackCooldown;
    
    public int GetUpgradeCost()
    {
        if (level >= MAX_LEVEL) return 0;
        
        switch (level)
        {
            case 1: return 50;
            case 2: return 120;
            default: return 0;
        }
    }

    public override void OnDayCycle()
    {
        /* do nothing */
    }

    public override void OnDestroy()
    {
        if (monkeys != null)
        {
            monkeys.FreeMonkeys();
        }
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