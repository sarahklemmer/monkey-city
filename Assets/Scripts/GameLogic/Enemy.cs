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
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem attackParticle;
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float particleStartSize = 0.1f; // Direct control of particle size
    [SerializeField] private int particleMaxCount = 10; // Max number of particles
    
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
        
        // Setup audio source if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
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
        if (WaveSpawner.instance != null && WaveSpawner.instance.IsPaused())
        {
            SetWalking(false);
            return;
        }

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
            
            // Play attack particle effect
            if (attackParticle != null)
            {
                // Spawn particle at attack point (between enemy and building)
                Vector3 attackPoint = Vector3.Lerp(transform.position, targetBuilding.transform.position, 0.7f);
                ParticleSystem particle = Instantiate(attackParticle, attackPoint, Quaternion.LookRotation(targetBuilding.transform.position - transform.position));
                
                // Modify particle system settings directly
                var main = particle.main;
                main.startSize = particleStartSize;
                main.maxParticles = particleMaxCount;
                
                particle.Play();
                Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
            }
            
            // Play attack sound
            if (attackSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(attackSound);
            }
            
            float blowback = targetBuilding.TakeDamage(attackDamage);
            if(blowback > 0) TakeDamage(blowback);
            
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
        
        // Play death particle effect
        if (deathParticle != null)
        {
            ParticleSystem particle = Instantiate(deathParticle, transform.position, Quaternion.identity);
            
            // Modify particle system settings directly
            var main = particle.main;
            main.startSize = particleStartSize;
            main.maxParticles = particleMaxCount;
            
            particle.Play();
            Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }
        
        // Play death sound
        if (deathSound != null)
        {
            // Create a temporary GameObject to play the sound since this object is being destroyed
            GameObject soundObject = new GameObject("DeathSound");
            soundObject.transform.position = transform.position;
            AudioSource tempSource = soundObject.AddComponent<AudioSource>();
            tempSource.clip = deathSound;
            tempSource.spatialBlend = 0.5f; // 3D sound
            tempSource.Play();
            Destroy(soundObject, deathSound.length);
        }
        
        // Trigger death animation if you have one
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        // Destroy immediately (or use delay if you want death animation: Destroy(gameObject, 0.5f))
        Destroy(gameObject);
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