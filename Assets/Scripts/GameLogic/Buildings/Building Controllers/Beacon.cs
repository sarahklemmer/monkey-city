using System.Collections.Generic;
using UnityEngine;

public class Beacon : BuildingBase
{
    private int level = 1;
    private const int MAX_LEVEL = 3;
    
    [Header("Beacon Settings")]
    [SerializeField] private float buffRadius = 5f;

    [SerializeField] private float attackSpeedBonus = 0.25f;
    [SerializeField] private float productionBonus = 0.20f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem upgradeEffect;
    [SerializeField] private ParticleSystem beaconAuraEffect;
    
    [Header("Range Indicator")]
    [SerializeField] private bool showRangeAlways = true;
    [SerializeField] private Color rangeColor = new Color(1f, 1f, 1f, 0.3f); 
    [SerializeField] private int circleSegments = 50;
    
    private LineRenderer rangeIndicator;
    private List<BuildingBase> buffedBuildings = new List<BuildingBase>();
    private HashSet<ArcherTower> buffedArchers = new HashSet<ArcherTower>();
    private HashSet<BananaFarm> buffedFarms = new HashSet<BananaFarm>();
    
    private static readonly int[] upgradeCosts = { 0, 75, 150, 250 };

    void Awake()
    {
        base.SharedAwakeBehavior();
        type = BuildingType.Beacon;
        monkeys = new(1);
        
        CreateRangeIndicator();
        
        Debug.Log("[Beacon] Awake completed, range indicator created");
    }

    void Start()
    {
        if (BuildingSoundManager.instance != null)
        {
            BuildingSoundManager.instance.PlayBuildingPlacedSound();
        }
        
        if (beaconAuraEffect != null)
        {
            beaconAuraEffect.Play();
        }
        
        if (rangeIndicator != null)
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = true;
            Debug.Log("[Beacon] Range indicator enabled");
        }
        else
        {
            Debug.LogWarning("[Beacon] Range indicator is NULL in Start!");
        }
    }

    protected override void UpdateBehavior()
    {
        base.UpdateBehavior();
        bananasPerDay = -2 * GetMonkeyCount() * level;
        if (GetMonkeyCount() > 0)
        {
            UpdateBuffedBuildings();
        }
        else
        {
            ClearAllBuffs();
        }
    }

    private void CreateRangeIndicator()
    {
        GameObject rangeObj = new GameObject("RangeIndicator");
        rangeObj.transform.SetParent(transform);
        rangeObj.transform.localPosition = Vector3.zero;
        
        rangeIndicator = rangeObj.AddComponent<LineRenderer>();
        rangeIndicator.useWorldSpace = false;
        rangeIndicator.loop = true;
        rangeIndicator.positionCount = circleSegments;
        rangeIndicator.startWidth = 0.15f;
        rangeIndicator.endWidth = 0.15f;
        
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
        
        UpdateRangeCircle();
        
        Debug.Log($"[Beacon] Range indicator created: segments={circleSegments}, radius={buffRadius}, color={rangeColor}");
    }

    private void UpdateRangeCircle()
    {
        if (rangeIndicator == null) return;
        
        float angleStep = 360f / circleSegments;
        
        for (int i = 0; i < circleSegments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * buffRadius;
            float z = Mathf.Sin(angle) * buffRadius;
            Vector3 position = new Vector3(x, 0.1f, z);
            rangeIndicator.SetPosition(i, position);
        }
    }

    private void UpdateBuffedBuildings()
    {
        BuildingBase[] allBuildings = FindObjectsByType<BuildingBase>(FindObjectsSortMode.None);
        
        HashSet<ArcherTower> currentArchers = new HashSet<ArcherTower>();
        HashSet<BananaFarm> currentFarms = new HashSet<BananaFarm>();
        
        foreach (var building in allBuildings)
        {
            if (building == this) continue;
            
            float distance = Vector3.Distance(transform.position, building.transform.position);
            
            if (distance <= buffRadius)
            {
                if (building is ArcherTower archer)
                {
                    currentArchers.Add(archer);
                    if (!buffedArchers.Contains(archer))
                    {
                        ApplyArcherBuff(archer);
                    }
                }
                else if (building is BananaFarm farm)
                {
                    currentFarms.Add(farm);
                    if (!buffedFarms.Contains(farm))
                    {
                        ApplyFarmBuff(farm);
                    }
                }
            }
        }
        
        buffedArchers.RemoveWhere(archer => 
        {
            if (!currentArchers.Contains(archer) || archer == null)
            {
                if (archer != null) RemoveArcherBuff(archer);
                return true;
            }
            return false;
        });
        
        buffedFarms.RemoveWhere(farm => 
        {
            if (!currentFarms.Contains(farm) || farm == null)
            {
                if (farm != null) RemoveFarmBuff(farm);
                return true;
            }
            return false;
        });
    }

    private void ApplyArcherBuff(ArcherTower archer)
    {
        Debug.Log($"Beacon buffing Archer Tower at {archer.transform.position}");
    }

    private void RemoveArcherBuff(ArcherTower archer)
    {
        Debug.Log($"Beacon removing buff from Archer Tower at {archer.transform.position}");
    }

    private void ApplyFarmBuff(BananaFarm farm)
    {
        Debug.Log($"Beacon buffing Banana Farm at {farm.transform.position}");
    }

    private void RemoveFarmBuff(BananaFarm farm)
    {
        Debug.Log($"Beacon removing buff from Banana Farm at {farm.transform.position}");
    }

    private void ClearAllBuffs()
    {
        foreach (var archer in buffedArchers)
        {
            if (archer != null) RemoveArcherBuff(archer);
        }
        buffedArchers.Clear();
        
        foreach (var farm in buffedFarms)
        {
            if (farm != null) RemoveFarmBuff(farm);
        }
        buffedFarms.Clear();
    }

    public void Upgrade()
    {
        if (level >= MAX_LEVEL)
        {
            Debug.Log($"Beacon is already at max level ({MAX_LEVEL})!");
            return;
        }
        
        level++;
        
        if (BuildingSoundManager.instance != null)
        {
            BuildingSoundManager.instance.PlayUpgradeSound();
        }
        
        switch (level)
        {
            case 2:
                buffRadius = 6.5f;
                attackSpeedBonus = 0.35f;
                productionBonus = 0.30f;
                break;
            case 3:
                buffRadius = 8f;
                attackSpeedBonus = 0.50f;
                productionBonus = 0.50f;
                break;
        }
        
        if (upgradeEffect != null)
        {
            upgradeEffect.Play();
        }
        
        UpdateRangeCircle();
        
        Debug.Log($"Beacon upgraded to level {level}! Radius: {buffRadius}, Attack Speed Bonus: {attackSpeedBonus * 100}%");
    }

    public void OnSelected()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = true;
        }
    }

    public void OnDeselected()
    {
        if (rangeIndicator != null && showRangeAlways)
        {
            rangeIndicator.enabled = true;
        }
    }

    public float GetAttackSpeedBonus() => attackSpeedBonus;
    public float GetProductionBonus() => productionBonus;
    public float GetBuffRadius() => buffRadius;
    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;
    
    public int GetUpgradeCost()
    {
        if (level >= MAX_LEVEL) return 0;
        return upgradeCosts[level];
    }

    public override void OnDayCycle()
    {
    }

    public override void OnDestroy()
    {
        ClearAllBuffs();
        
        if (monkeys != null)
        {
            monkeys.FreeMonkeys();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, buffRadius);
    }
}