using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float arcHeight = 2f;
    [SerializeField] private float lifetime = 3f;
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private EnemyAttacker targetEnemy;
    private float damage;
    private float journeyLength;
    private float startTime;
    private bool isInitialized = false;
    private bool hasDealtDamage = false;

    public void Initialize(EnemyAttacker target, float dmg)
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        startPosition = transform.position;
        targetEnemy = target;
        targetPosition = target.transform.position;
        damage = dmg;
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        startTime = Time.time;
        isInitialized = true;
        
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isInitialized) return;

        if (WaveSpawner.instance != null && WaveSpawner.instance.IsPaused())
        {
            return;
        }

        if (targetEnemy != null && targetEnemy.GetCurrentHealth() > 0)
        {
            targetPosition = targetEnemy.transform.position;
            journeyLength = Vector3.Distance(startPosition, targetPosition);
        }

        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / journeyLength;

        if (fractionOfJourney >= 0.95f && !hasDealtDamage)
        {
            DealDamage();
        }

        if (fractionOfJourney >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
        
        float arc = arcHeight * Mathf.Sin(fractionOfJourney * Mathf.PI);
        currentPos.y += arc;
        
        transform.position = currentPos;
        
        if (fractionOfJourney < 0.99f)
        {
            Vector3 nextPos = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney + 0.01f);
            nextPos.y += arcHeight * Mathf.Sin((fractionOfJourney + 0.01f) * Mathf.PI);
            transform.LookAt(nextPos);
        }
    }

    private void DealDamage()
    {
        if (hasDealtDamage) return;
        hasDealtDamage = true;

        if (targetEnemy != null && targetEnemy.GetCurrentHealth() > 0)
        {
            targetEnemy.TakeDamage(damage);
        }
    }
}