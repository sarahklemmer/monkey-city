using UnityEngine;

public class ChickenEnemy : EnemyAttacker
{
    [Header("Chicken Stats Override")]
    [SerializeField] private float chickenMoveSpeed = 4f;
    [SerializeField] private float chickenMaxHealth = 20f;
    [SerializeField] private float chickenAttackDamage = 5f;
    [SerializeField] private float chickenAttackCooldown = 0.8f;
    
    void Start()
    {
        SetMoveSpeed(chickenMoveSpeed);
        SetMaxHealth(chickenMaxHealth);
        SetAttackDamage(chickenAttackDamage);
        SetAttackCooldown(chickenAttackCooldown);
    }
}