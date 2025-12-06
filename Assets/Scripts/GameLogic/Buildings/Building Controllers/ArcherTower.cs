using System;
using UnityEngine.Assertions;
using UnityEngine;
using System.Collections.Generic;

public enum ArcherTowerType
{
    Base,
    SniperMonkey,
    TackSprayer
}

public class ArcherTower : BuildingBase
{
    public int level {get; private set; } = 1;
    public const int CHOOSE_PATH_LEVEL = 3;
    public const int MAX_LEVEL = 6;

    ArcherTowerType archerType = ArcherTowerType.Base;
    
    private float attackRange = 5f;
    private float attackDamage = 10f;
    private float attackMultiplier = 1f;
    private float attackCooldown = 1f; 
    
    [SerializeField] GameObject arrowPrefab;
    
    [SerializeField] ParticleSystem upgradeEffect;
    
    private Transform firePoint;
    private float lastAttackTime;
    private EnemyAttacker targetEnemy;

    // Range indicator components
    private LineRenderer rangeIndicator;
    [SerializeField] private Color rangeColor = new Color(0.5f, 0.8f, 1f, 0.15f);
    [SerializeField] private int circleSegments = 50;
    [SerializeField] private Vector3 circleOffset = Vector3.zero;

    void Awake()
    {
        base.SharedAwakeBehavior();
        
        type = BuildingType.ArcherTower;
        canNeverBeUpgraded = false;
        monkeys = new(1);
    
        // Find and stop any looping particle systems
        ParticleSystem[] allParticles = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in allParticles)
        {
            if (ps != upgradeEffect && ps.main.loop)
            {
                ps.Stop();
                ps.Clear();
            }
        }
        
        CreateRangeIndicator();
    }

    void Start()
    {
        BuildingSoundManager.instance.PlayBuildingPlacedSound();
        WaveSpawner.instance.OnFirstTowerPlaced(this);
    }

    private void CreateRangeIndicator()
    {
        GameObject rangeObj = new GameObject("RangeIndicator");
        rangeObj.transform.SetParent(transform);
        rangeObj.transform.localPosition = circleOffset;
        
        rangeIndicator = rangeObj.AddComponent<LineRenderer>();
        rangeIndicator.useWorldSpace = false;
        rangeIndicator.loop = true;
        rangeIndicator.positionCount = circleSegments;
        rangeIndicator.startWidth = 0.08f;
        rangeIndicator.endWidth = 0.08f;
        
        Material mat = new Material(Shader.Find("Sprites/Default"));
        if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
        {
            mat = new Material(Shader.Find("Unlit/Color"));
        }
        if (mat.shader == null || mat.shader.name == "Hidden/InternalErrorShader")
        {
            mat = new Material(Shader.Find("Particles/Standard Unlit"));
        }
        
        rangeIndicator.material = mat;
        rangeIndicator.startColor = rangeColor;
        rangeIndicator.endColor = rangeColor;
        rangeIndicator.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rangeIndicator.receiveShadows = false;
        rangeIndicator.enabled = false;

        UpdateRangeCircle();
    }

    private void UpdateRangeCircle()
    {
        float angleStep = 360f / circleSegments;
        
        for (int i = 0; i < circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * attackRange;
            float z = Mathf.Sin(angle) * attackRange;
            Vector3 position = new Vector3(x, 0.1f, z);
            rangeIndicator.SetPosition(i, position);
        }
    }

    private float GetEffectiveAttackCooldown()
    {
        float baseCooldown = attackCooldown;
        
        // Check for nearby beacons
        Beacon[] beacons = FindObjectsByType<Beacon>(FindObjectsSortMode.None);
        float bestBonus = 0f;
        
        foreach (var beacon in beacons)
        {
            if (beacon.GetMonkeyCount() <= 0) continue;
            
            float distance = Vector3.Distance(transform.position, beacon.transform.position);
            if (distance <= beacon.GetBuffRadius())
            {
                bestBonus = Mathf.Max(bestBonus, beacon.GetAttackSpeedBonus());
            }
        }
        
        // Apply bonus (reduce cooldown = faster attacks)
        return baseCooldown * (1f - bestBonus);
    }

    void Update()
    {
        bananasPerDay = ((int)Math.Pow(5, level - 1)) * GetMonkeyCount() * -1;
        attackDamage = AllArcherTowerInfo.instance.GetDamagePerAttack();

        if (monkeys.count == 0)
        {
            if (targetEnemy != null) targetEnemy = null;
            return;
        }

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

        if (Time.time - lastAttackTime >= GetEffectiveAttackCooldown())
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
        
        if (arrowPrefab == null)
        {
            Debug.LogError("[ArcherTower] Arrow prefab is NULL!");
            return;
        }
        
        GameObject arrow = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        
        if (arrowScript != null && targetEnemy != null)
        {
            arrowScript.Initialize(targetEnemy, attackDamage * attackMultiplier);
        }
    }

    public override void OnSelectOrView() => rangeIndicator.enabled = true;
    public override void OnDeslectOrStopViewing() => rangeIndicator.enabled = false; 

    public override bool CanUpgrade() 
    {
        TreeOfLife tree = BuildingManager.instance.GetTreeOfLife() as TreeOfLife;
        if (tree == null) return false;

        if (canNeverBeUpgraded) return false;
        if (BananaManager.instance.GetBananas() < GetUpgradeCost()) return false;

        if (level >= 2 && tree.GetLevel() < 2) return false;
        if (NeedToSetType()) return false;

        return true;
    }

    public override bool AttemptUpgrade()
    {
        if(!CanUpgrade()) return false;
        BananaManager.instance.AddBananas(-GetUpgradeCost());
        Upgrade();
        return true;
    }

    public override int GetUpgradeCost()
    {        
        switch (level)
        {
            case 1: return 20;
            case 2: return 50;
            case 3: return 100;
            case 4:
            case 5: return 250;
            default: return 0;
        }
    }

    void Upgrade()
    {
        Assert.IsFalse(level >= MAX_LEVEL, "upgrading when we're already at or above? max level!");
        BuildingSoundManager.instance.PlayUpgradeSound();

        if(level == 1)
        {
            attackCooldown = 0.7f;
        } else if(level == 2)
        {
            attackRange = 6.5f;
            attackCooldown = 0.5f;
        } 
        else if(level >= CHOOSE_PATH_LEVEL && archerType == ArcherTowerType.SniperMonkey) UpgradeSniperMonkey();
        else if(level >= CHOOSE_PATH_LEVEL && archerType == ArcherTowerType.TackSprayer) UpgradeTackSprayer();
        else
        {
            // basically just a std::unreachable
            Assert.IsTrue(false, "somehow upgrading with base archertower after passing path choose level");
        }

        ArcherTowerUpgradeEffects modelUpgradeEffects = GetComponent<ArcherTowerUpgradeEffects>();
        modelUpgradeEffects.Upgrade(archerType, level);
        upgradeEffect.Play();
        
        level++;

        UpdateRangeCircle();
        if(level == MAX_LEVEL) canNeverBeUpgraded = true;
    }

    void UpgradeSniperMonkey()
    {
        Assert.IsTrue(level >= CHOOSE_PATH_LEVEL, "upgrading sniper monkey too early");
        if(level == CHOOSE_PATH_LEVEL)
        {
            attackRange *= 2;
            attackCooldown *= 3;
            attackMultiplier += 4f;
        } else {
            attackRange += 1;
            attackMultiplier += 1f;
        }
    }

    void UpgradeTackSprayer()
    {
        Assert.IsTrue(level >= CHOOSE_PATH_LEVEL, "upgrading tack sprayer too early");
        if(level == CHOOSE_PATH_LEVEL)
        {
            attackRange /= 2;
            attackCooldown /= 3;
        } else attackCooldown /= 2;

        attackMultiplier += 0.5f;
    }

    public override string GetUpgradeText()
    {
        switch(level)
        {
            case 1: return $"Upgrade Cost: {GetUpgradeCost()} Bananas\n Next Upgrade: Faster shooting";
            case 2: return (level >= 2 && (BuildingManager.instance.GetTreeOfLife() as TreeOfLife).GetLevel() < 2) ?  "Requires Tree of Life Level 2" : $"Upgrade Cost: {GetUpgradeCost()} Bananas\n Next Upgrade: Increased range & faster shooting";
            case 3: return $"Upgrade Cost: {GetUpgradeCost()} Bananas\n Next Upgrade: Pick a path";
            case 4: 
            case 5: return $"Upgrade Cost: {GetUpgradeCost()} Bananas\n " + (archerType == ArcherTowerType.SniperMonkey ? "higher damage and attack range" : "much higher attack speed");
            default: return "Max level";
        }
    }

    public bool NeedToSetType() => archerType == ArcherTowerType.Base && level >= CHOOSE_PATH_LEVEL;

    public void SelectType(ArcherTowerType ty)
    {
        Assert.AreEqual(archerType, ArcherTowerType.Base, "setting type when one has been selected");
        Assert.AreNotEqual(ty, ArcherTowerType.Base, "setting type to baes");
        archerType = ty;
    }

    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    public float GetAttackRange() => attackRange;
    public float GetAttackCooldown() => attackCooldown;
    public override string GetDescription() =>                     
                    $"Level: {GetLevel()}\n" +
                    $"Range: {GetAttackRange():F1}m\n" +
                    $"Damage: {AllArcherTowerInfo.instance.GetDamagePerAttack():F1}\n" +
                    $"Attack Speed: {GetAttackCooldown():F2}s\n" +
                    $"Monkeys: {GetMonkeyCount()}/{GetMonkeyCapacity()}";

    public override void OnDayCycle() {}
    public override void OnDestroy() => monkeys.FreeMonkeys();

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
    
    private HashSet<Beacon> beaconBuffs = new HashSet<Beacon>();

    public void AddBeaconBuff(Beacon beacon)
    {
        beaconBuffs.Add(beacon);
    }

    public void RemoveBeaconBuff(Beacon beacon)
    {
        beaconBuffs.Remove(beacon);
    }

    public float GetTotalAttackSpeedBonus()
    {
        float bonus = 0f;
        foreach (var beacon in beaconBuffs)
        {
            if (beacon != null && beacon.GetMonkeyCount() > 0)
            {
                bonus += beacon.GetAttackSpeedBonus();
            }
        }
        return bonus;
    }
}