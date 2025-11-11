using UnityEngine;
using System.Collections;

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
    
    [Header("Damage Flash")]
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private Color flashColor = Color.red;
    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private bool isFlashing = false;
    
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
        
        // Get all renderers and store original materials
        SetupFlashEffect();
    }

    private void SetupFlashEffect()
    {
        renderers = GetComponentsInChildren<Renderer>();
        
        if (renderers.Length > 0)
        {
            originalMaterials = new Material[renderers.Length][];
            
            for (int i = 0; i < renderers.Length; i++)
            {
                originalMaterials[i] = renderers[i].materials;
            }
        }
        else
        {
            Debug.LogWarning($"No Renderers found on {gameObject.name} - damage flash won't work!");
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
        
        // Flash red when taking damage
        if (!isFlashing)
        {
            StartCoroutine(FlashRed());
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        if (renderers == null || renderers.Length == 0)
        {
            yield break;
        }
        
        isFlashing = true;
        
        // Create flash materials
        Material[][] flashMaterials = new Material[renderers.Length][];
        
        for (int i = 0; i < renderers.Length; i++)
        {
            flashMaterials[i] = new Material[originalMaterials[i].Length];
            
            for (int j = 0; j < originalMaterials[i].Length; j++)
            {
                // Create a new material instance with flash color
                flashMaterials[i][j] = new Material(originalMaterials[i][j]);
                flashMaterials[i][j].color = flashColor;
                
                // If using Standard shader, also set emission
                if (flashMaterials[i][j].HasProperty("_EmissionColor"))
                {
                    flashMaterials[i][j].EnableKeyword("_EMISSION");
                    flashMaterials[i][j].SetColor("_EmissionColor", flashColor * 0.5f);
                }
            }
            
            renderers[i].materials = flashMaterials[i];
        }
        
        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);
        
        // Restore original materials
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].materials = originalMaterials[i];
        }
        
        isFlashing = false;
    }

    private void Die()
    {
        // Stop any ongoing flash
        StopAllCoroutines();
        
        // Restore materials before death
        if (renderers != null && originalMaterials != null)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    renderers[i].materials = originalMaterials[i];
                }
            }
        }
        
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