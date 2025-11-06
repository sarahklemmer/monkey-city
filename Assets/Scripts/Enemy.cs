using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float recoilDistance = 1f;
    [SerializeField] private float recoilDuration = 0.2f;
    
    private BuildingHealth targetBuilding;
    private float lastAttackTime;
    private bool isRecoiling = false;
    private Vector3 recoilStartPos;
    private Vector3 recoilTargetPos;
    private float recoilTimer;

    void Update()
    {
        if (isRecoiling)
        {
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
            FindNearestBuilding();
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, targetBuilding.transform.position);

        if (targetBuilding == null || targetBuilding.GetCurrentHealth() <= 0)
        {
            targetBuilding = null;
            return;
        }

        if (distanceToTarget > attackRange)
        {
            MoveTowardTarget();
        }
        else
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackBuilding();
                lastAttackTime = Time.time;
            }
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
            targetBuilding.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} dealt {attackDamage} damage to {targetBuilding.gameObject.name}");
            
            Vector3 directionAwayFromTarget = (transform.position - targetBuilding.transform.position).normalized;
            recoilStartPos = transform.position;
            recoilTargetPos = transform.position + directionAwayFromTarget * recoilDistance;
            recoilTimer = 0f;
            isRecoiling = true;
        }
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