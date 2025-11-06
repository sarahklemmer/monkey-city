using UnityEngine;

// Example enemy script that attacks buildings
public class EnemyAttacker : MonoBehaviour
{
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 2f;
    
    private BuildingHealth targetBuilding;
    private float lastAttackTime;

    void Update()
    {
        if (targetBuilding == null)
        {
            FindNearestBuilding();
            return;
        }

        // Check if building is still alive and in range
        if (targetBuilding == null || Vector3.Distance(transform.position, targetBuilding.transform.position) > attackRange)
        {
            targetBuilding = null;
            return;
        }

        // Attack on cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            AttackBuilding();
            lastAttackTime = Time.time;
        }
    }

    private void FindNearestBuilding()
    {
        BuildingHealth[] buildings = FindObjectsByType<BuildingHealth>(FindObjectsSortMode.None);
        float closestDist = Mathf.Infinity;
        BuildingHealth closest = null;

        foreach (var building in buildings)
        {
            float dist = Vector3.Distance(transform.position, building.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = building;
            }
        }

        if (closestDist <= attackRange)
        {
            targetBuilding = closest;
        }
    }

    private void AttackBuilding()
    {
        if (targetBuilding != null)
        {
            targetBuilding.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} dealt {attackDamage} damage to {targetBuilding.gameObject.name}");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}