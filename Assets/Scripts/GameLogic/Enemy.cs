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

    [Header("Audio Clips")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip deathSound;

    [Header("Audio Volumes")]
    [SerializeField, Range(0f, 1f)] private float attackVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float deathVolume = 1f;

    [SerializeField] private float particleStartSize = 0.1f;
    [SerializeField] private int particleMaxCount = 10;

    [Header("Damage Flash")]
    [SerializeField] private float flashDuration = 0.15f;
    [SerializeField] private Color flashColor = Color.red;
    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private bool isFlashing = false;

    [Header("Health Bar")]
    [SerializeField] private EnemyHealthBar healthBar;

    private BuildingHealth targetBuilding;
    private BuildingHealth priorityTarget; // First tower for tutorial wave
    private float lastAttackTime;
    private bool isRecoiling = false;
    private Vector3 recoilStartPos;
    private Vector3 recoilTargetPos;
    private float recoilTimer;
    private float currentHealth;
    private bool isWalking = false;

    private AudioSource attackSource;
    private AudioSource deathSource;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponentInChildren<Animator>();

        var existingSources = GetComponentsInChildren<AudioSource>(true);
        foreach (var s in existingSources)
            s.enabled = false;

        attackSource = gameObject.AddComponent<AudioSource>();
        deathSource  = gameObject.AddComponent<AudioSource>();

        attackSource.playOnAwake = false;
        deathSource.playOnAwake  = false;
        attackSource.loop = false;
        deathSource.loop  = false;

        attackSource.spatialBlend = 0f;
        deathSource.spatialBlend  = 0f;

        if (healthBar == null)
            healthBar = GetComponentInChildren<EnemyHealthBar>();

        SetupFlashEffect();
    }

    private void SetupFlashEffect()
    {
        renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            originalMaterials = new Material[renderers.Length][];
            for (int i = 0; i < renderers.Length; i++)
                originalMaterials[i] = renderers[i].materials;
        }
    }

    // Called by WaveSpawner for tutorial wave
    public void SetPriorityTargetToFirstTower()
    {
        // Find all towers (ArcherTower or similar building types)
        BuildingHealth[] allBuildings = FindObjectsByType<BuildingHealth>(FindObjectsSortMode.None);
        BuildingHealth firstTower = null;
        float closestDistance = float.MaxValue;
        
        foreach (BuildingHealth building in allBuildings)
        {
            if (building.GetCurrentHealth() <= 0) continue;
            
            // Check if it's an ArcherTower (or add other tower types here)
            ArcherTower tower = building.GetComponent<ArcherTower>();
            if (tower != null)
            {
                float distance = Vector3.Distance(transform.position, building.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    firstTower = building;
                }
            }
        }
        
        if (firstTower != null)
        {
            priorityTarget = firstTower;
            targetBuilding = firstTower;
            Debug.Log("Tutorial chimp targeting first tower!");
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

            if (t >= 1f) isRecoiling = false;
            else
            {
                float recoilCurve = Mathf.Sin(t * Mathf.PI);
                transform.position = Vector3.Lerp(recoilStartPos, recoilTargetPos, recoilCurve);
            }
            return;
        }

        // If we have a priority target (first tower), stick to it until it's destroyed
        if (priorityTarget != null)
        {
            if (priorityTarget.GetCurrentHealth() <= 0)
            {
                priorityTarget = null;
                targetBuilding = null;
            }
            else
            {
                targetBuilding = priorityTarget;
            }
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
        transform.LookAt(new Vector3(
            targetBuilding.transform.position.x,
            transform.position.y,
            targetBuilding.transform.position.z));
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
        if (targetBuilding == null) return;

        if (animator != null)
            animator.SetTrigger("Attack");

        if (attackParticle != null)
        {
            Vector3 attackPoint = Vector3.Lerp(transform.position, targetBuilding.transform.position, 0.7f);
            ParticleSystem particle = Instantiate(
                attackParticle,
                attackPoint,
                Quaternion.LookRotation(targetBuilding.transform.position - transform.position));

            var main = particle.main;
            main.startSize = particleStartSize;
            main.maxParticles = particleMaxCount;

            particle.Play();
            Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }

        if (attackSound != null && attackSource != null)
        {
            attackSource.PlayOneShot(attackSound, attackVolume);
        }

        float blowback = targetBuilding.TakeDamage(attackDamage);
        if (blowback > 0) TakeDamage(blowback);

        Vector3 directionAwayFromTarget = (transform.position - targetBuilding.transform.position).normalized;
        recoilStartPos = transform.position;
        recoilTargetPos = transform.position + directionAwayFromTarget * recoilDistance;
        recoilTimer = 0f;
        isRecoiling = true;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
            healthBar.ForceUpdate();

        if (!isFlashing)
            StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator FlashRed()
    {
        if (renderers == null || renderers.Length == 0)
            yield break;

        isFlashing = true;
        Material[][] flashMaterials = new Material[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            flashMaterials[i] = new Material[originalMaterials[i].Length];

            for (int j = 0; j < originalMaterials[i].Length; j++)
            {
                flashMaterials[i][j] = new Material(originalMaterials[i][j]);
                flashMaterials[i][j].color = flashColor;

                if (flashMaterials[i][j].HasProperty("_EmissionColor"))
                {
                    flashMaterials[i][j].EnableKeyword("_EMISSION");
                    flashMaterials[i][j].SetColor("_EmissionColor", flashColor * 0.5f);
                }
            }

            renderers[i].materials = flashMaterials[i];
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].materials = originalMaterials[i];

        isFlashing = false;
    }

    private void Die()
    {
        StopAllCoroutines();

        if (renderers != null && originalMaterials != null)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    renderers[i].materials = originalMaterials[i];
            }
        }

        if (deathParticle != null)
        {
            ParticleSystem particle = Instantiate(deathParticle, transform.position, Quaternion.identity);

            var main = particle.main;
            main.startSize = particleStartSize;
            main.maxParticles = particleMaxCount;

            particle.Play();
            Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }

        if (deathSound != null)
        {
            GameObject soundObject = new GameObject("EnemyDeathSound");
            soundObject.transform.position = transform.position;

            AudioSource temp = soundObject.AddComponent<AudioSource>();
            temp.spatialBlend = 0f;
            temp.playOnAwake = false;
            temp.loop = false;

            temp.PlayOneShot(deathSound, deathVolume);

            Destroy(soundObject, deathSound.length + 0.1f);
        }

        if (animator != null)
            animator.SetTrigger("Die");

        Destroy(gameObject);
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth / maxHealth;

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