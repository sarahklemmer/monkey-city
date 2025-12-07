using UnityEngine;
using System.Collections;

public class TigerBossEnemy : EnemyAttacker
{
    [Header("Boss Stats")]
    [SerializeField] private float bossAttackDamage = 25f;
    [SerializeField] private float bossAttackCooldown = 1.5f;
    [SerializeField] private float bossAttackRange = 3f;
    [SerializeField] private float bossMoveSpeed = 3f;
    [SerializeField] private float bossMaxHealth = 200f;
    
    [Header("Regeneration")]
    [SerializeField] private float regenAmount = 5f;
    [SerializeField] private float regenInterval = 3f;
    
    [Header("Boss Effects")]
    [SerializeField] private ParticleSystem regenParticle;
    
    [Header("Boss Audio")]
    [SerializeField] private AudioClip roarSound;
    [SerializeField, Range(0f, 1f)] private float roarVolume = 1f;
    
    private float lastRegenTime;
    private AudioSource roarSource;
    
    protected override void Awake()
    {
        attackDamage = bossAttackDamage;
        attackCooldown = bossAttackCooldown;
        attackRange = bossAttackRange;
        moveSpeed = bossMoveSpeed;
        maxHealth = bossMaxHealth;
        
        base.Awake();
        
        roarSource = gameObject.AddComponent<AudioSource>();
        roarSource.playOnAwake = false;
        roarSource.loop = false;
        roarSource.spatialBlend = 0f;
        
        lastRegenTime = Time.time;
        
        if (roarSound != null && roarSource != null)
        {
            roarSource.PlayOneShot(roarSound, roarVolume);
        }
    }
    
    protected override void Update()
    {
        if (WaveSpawner.instance != null && WaveSpawner.instance.IsPaused())
        {
            SetWalking(false);
            return;
        }
        
        if (Time.time - lastRegenTime >= regenInterval)
        {
            RegenerateHealth();
            lastRegenTime = Time.time;
        }
        
        base.Update();
    }
    
    private void RegenerateHealth()
    {
        float currentHP = GetCurrentHealth();
        float maxHP = GetMaxHealth();
        
        if (currentHP >= maxHP)
            return;
        
        Heal(regenAmount);
        
        if (regenParticle != null)
        {
            ParticleSystem particle = Instantiate(regenParticle, transform.position + Vector3.up, Quaternion.identity);
            var main = particle.main;
            main.startSize = particleStartSize;
            main.maxParticles = particleMaxCount;
            particle.Play();
            Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }
    }
    
    protected override void AttackBuilding()
    {
        if (targetBuilding == null) return;
        
        if (animator != null && HasAnimatorParameter("Attack"))
            animator.SetTrigger("Attack");
        
        if (attackParticle != null)
        {
            Vector3 attackPoint = Vector3.Lerp(transform.position, targetBuilding.transform.position, 0.7f);
            ParticleSystem particle = Instantiate(
                attackParticle,
                attackPoint,
                Quaternion.LookRotation(targetBuilding.transform.position - transform.position));
            
            var main = particle.main;
            main.startSize = particleStartSize * 1.5f;
            main.maxParticles = particleMaxCount * 2;
            
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
    
    protected override void Die()
    {
        if (deathParticle != null)
        {
            ParticleSystem particle = Instantiate(deathParticle, transform.position, Quaternion.identity);
            var main = particle.main;
            main.startSize = particleStartSize * 3f;
            main.maxParticles = particleMaxCount * 3;
            particle.Play();
            Destroy(particle.gameObject, particle.main.duration + particle.main.startLifetime.constantMax);
        }
        
        base.Die();
    }
    
    private bool HasAnimatorParameter(string paramName)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        if (targetBuilding != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetBuilding.transform.position);
        }
    }
}