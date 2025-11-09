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

    void Awake()
    {
        base.SharedAwakeBehavior();
        building = new(BuildingType.ArcherTower);
    }

    void Update()
    {
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
        
        Debug.Log($"Archer Tower upgraded to level {level}!");
    }

    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    public float GetAttackRange() => attackRange;
    public float GetAttackDamage() => attackDamage;
    public float GetAttackCooldown() => attackCooldown;

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