using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [Header("Combat Stats")]
    private float attackDamage = 10f;
    private float attackCooldown = 1f;
    private float attackRange = 2f;
    private float moveSpeed = 2f;
    private float recoilDistance = 1f;
    private float recoilDuration = 0.2f;
    private float maxHealth = 40f;
    
    [Header("Animation")]
    private Animator animator;
    
    private BuildingHealth targetBuilding;
    private float lastAttackTime;
    private bool isRecoiling = false;
    private Vector3 recoilStartPos;
    private Vector3 recoilTargetPos;
    private float recoilTimer;
    private float currentHealth;
    private bool isWalking = false;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();
        
        if (animator == null)
        {
            Debug.LogWarning($"No Animator found on {gameObject.name} or its children!");
        }
    }

    void Update()
    {
        if (isRecoiling)
        {
            SetWalking(false);
            recoilTimer += Time.deltaTime;
            float t = recoilTimer / recoilDuration;
            
            if (t >= 1f)
            {
                isRecoiling = false;
            }
            else
            {
                float recoilCurve = Mathf.Sin(t * Mathf.PI);
                transform.position = Vector3.Lerp(recoilStartPos, recoilTargetPos, recoilCurve);
            }
            return;
        }

        if (targetBuilding == null)
        {
            SetWalking(false);
            FindNearestBuilding();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, targetBuilding.transform.position);

        if (targetBuilding == null || targetBuilding.GetCurrentHealth() <= 0)
        {
            targetBuilding = null;
            SetWalking(false);
            return;
        }

        if (distanceToTarget > attackRange)
        {
            SetWalking(true);
            MoveTowardTarget();
        }
        else
        {
            SetWalking(false);
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackBuilding();
                lastAttackTime = Time.time;
            }
        }
    }

    private void SetWalking(bool walking)
    {
        if (animator != null && isWalking != walking)
        {
            isWalking = walking;
            animator.SetBool("IsWalking", walking);
        }
    }

    private void MoveTowardTarget()
    {
        if (targetBuilding == null) return;

        Vector3 direction = (targetBuilding.transform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(targetBuilding.transform.position.x, transform.position.y, targetBuilding.transform.position.z));
    }

    private void FindNearestBuilding()
    {
        BuildingHealth[] buildings = FindObjectsByType<BuildingHealth>(FindObjectsSortMode.None);
        float closestDist = Mathf.Infinity;
        BuildingHealth closest = null;

        foreach (var building in buildings)
        {
            if (building.GetCurrentHealth() <= 0) continue;
            float dist = Vector3.Distance(transform.position, building.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = building;
            }
        }

        targetBuilding = closest;
    }

    private void AttackBuilding()
    {
        if (targetBuilding != null)
        {
            // Trigger attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            targetBuilding.TakeDamage(attackDamage);
            
            Vector3 directionAwayFromTarget = (transform.position - targetBuilding.transform.position).normalized;
            recoilStartPos = transform.position;
            recoilTargetPos = transform.position + directionAwayFromTarget * recoilDistance;
            recoilTimer = 0f;
            isRecoiling = true;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Trigger death animation if you have one
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (Tutorial.instance.tutorialActive) Tutorial.instance.ChimpKilledByTower();
        // Destroy after a short delay to let death animation play
        Destroy(gameObject, 0.5f);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (targetBuilding != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetBuilding.transform.position);
        }
    }
}