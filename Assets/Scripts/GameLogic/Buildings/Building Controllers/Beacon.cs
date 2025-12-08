using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    [SerializeField] private Color rangeColor = new Color(1f, 1f, 1f, 0.3f); 
    [SerializeField] private int circleSegments = 50;
    
    private LineRenderer rangeIndicator;
    private List<BuildingBase> buffedBuildings = new List<BuildingBase>();
    private HashSet<ArcherTower> buffedArchers = new HashSet<ArcherTower>();
    private HashSet<BananaFarm> buffedFarms = new HashSet<BananaFarm>();
    
    private static readonly int[] upgradeCosts = { 0, 75, 150, 250 };

    private BeaconUpgradeEffects upgradeEffects;

    void Awake()
    {
        base.SharedAwakeBehavior();
        type = BuildingType.Beacon;
        canNeverBeUpgraded = false;
        monkeys = new(1);
        
        upgradeEffects = GetComponent<BeaconUpgradeEffects>();
        
        CreateRangeIndicator();
    }

    void Start()
    {
        BuildingSoundManager.instance.PlayBuildingPlacedSound();
        
        if (beaconAuraEffect != null) beaconAuraEffect.Play();
        Assert.IsNotNull(rangeIndicator, "rangeindicator is null in beacon");
        rangeIndicator.enabled = false;
        
        Debug.Log($"[Beacon] Started with radius {buffRadius}, attack bonus {attackSpeedBonus * 100}%, production bonus {productionBonus * 100}%");
    }

    void Update()
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
        
        int towersInRange = 0;
        int farmsInRange = 0;
        
        foreach (var building in allBuildings)
        {
            if (building == this) continue;
            
            float distance = Vector3.Distance(transform.position, building.transform.position);
            
            if (distance <= buffRadius)
            {
                if (building is ArcherTower archer)
                {
                    towersInRange++;
                    currentArchers.Add(archer);
                    if (!buffedArchers.Contains(archer))
                    {
                        Debug.Log($"[Beacon] Adding buff to ArcherTower at distance {distance:F2}. Bonus: {attackSpeedBonus * 100}%");
                        ApplyArcherBuff(archer);
                    }
                }
                else if (building is BananaFarm farm)
                {
                    farmsInRange++;
                    currentFarms.Add(farm);
                    if (!buffedFarms.Contains(farm))
                    {
                        Debug.Log($"[Beacon] Adding buff to BananaFarm at distance {distance:F2}. Bonus: {productionBonus * 100}%");
                        ApplyFarmBuff(farm);
                    }
                }
            }
        }
        
        if (Time.frameCount % 300 == 0) // Log every ~5 seconds
        {
            Debug.Log($"[Beacon] Currently buffing {buffedArchers.Count} towers and {buffedFarms.Count} farms. In range: {towersInRange} towers, {farmsInRange} farms. Monkey count: {GetMonkeyCount()}");
        }
        
        buffedArchers.RemoveWhere(archer => 
        {
            if (!currentArchers.Contains(archer) || archer == null)
            {
                if (archer != null) 
                {
                    Debug.Log($"[Beacon] Removing buff from ArcherTower (out of range or destroyed)");
                    RemoveArcherBuff(archer);
                }
                return true;
            }
            return false;
        });
        
        buffedFarms.RemoveWhere(farm => 
        {
            if (!currentFarms.Contains(farm) || farm == null)
            {
                if (farm != null) 
                {
                    Debug.Log($"[Beacon] Removing buff from BananaFarm (out of range or destroyed)");
                    RemoveFarmBuff(farm);
                }
                return true;
            }
            return false;
        });
    }

    private void ApplyArcherBuff(ArcherTower archer)
    {
        archer.AddBeaconBuff(this);
        Debug.Log($"[Beacon] ApplyArcherBuff called. Archer now has {archer.GetTotalAttackSpeedBonus() * 100}% bonus");
    }

    private void RemoveArcherBuff(ArcherTower archer)
    {
        archer.RemoveBeaconBuff(this);
    }

    private void ApplyFarmBuff(BananaFarm farm)
    {
        farm.AddBeaconBuff(this);
        Debug.Log($"[Beacon] ApplyFarmBuff called. Farm now has {farm.GetTotalProductionBonus() * 100}% bonus");
    }

    private void RemoveFarmBuff(BananaFarm farm)
    {
        farm.RemoveBeaconBuff(this);
    }

    private void ClearAllBuffs()
    {
        if (buffedArchers.Count > 0 || buffedFarms.Count > 0)
        {
            Debug.Log($"[Beacon] Clearing all buffs (no monkey assigned)");
        }
        
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

    public float GetAttackSpeedBonus() => attackSpeedBonus;
    public float GetProductionBonus() => productionBonus;
    public float GetBuffRadius() => buffRadius;
    public int GetLevel() => level;
    public bool IsMaxLevel() => level >= MAX_LEVEL;

    public override string GetDescription()
    {
        string monkeyStatus = GetMonkeyCount() > 0 ? "ACTIVE" : "INACTIVE";
        int buffedCount = buffedArchers.Count + buffedFarms.Count;
        
        string statsText = $"<size=14><b>Beacon</b> Lv{level}/{MAX_LEVEL} - {monkeyStatus}\n" +
                          $"Buffing: {buffedCount} buildings\n" +
                          $"Range: {buffRadius}m\n" +
                          $"<b>Buffs:</b>" +
                          $"Towers: +{attackSpeedBonus * 100:F0}% speed\n" +
                          $"Farms: +{productionBonus * 100:F0}% production\n" +
                          $"Upkeep: {Mathf.Abs(bananasPerDay)}/day";
        
        if (level < MAX_LEVEL)
        {
            statsText += $"\n<b>Next:</b> ";
            switch (level)
            {
                case 1:
                    statsText += $"6.5m, +35% spd, +30% prod";
                    break;
                case 2:
                    statsText += $"8m, +50% spd, +50% prod";
                    break;
            }
        }
        
        statsText += "</size>";
        return statsText;
    }

    public override void OnSelectOrView() 
    {
        rangeIndicator.enabled = true;
        if (upgradeEffects != null)
        {
            upgradeEffects.ShowSelectionIndicator();
        }
    }

    public override void OnDeslectOrStopViewing() 
    {
        rangeIndicator.enabled = false;
        if (upgradeEffects != null)
        {
            upgradeEffects.HideSelectionIndicator();
        }
    }

    public override bool CanUpgrade() => GetUpgradeCost() <= BananaManager.instance.GetBananas() && !canNeverBeUpgraded;

    public override bool AttemptUpgrade()
    {
        if(!CanUpgrade()) return false;
        BananaManager.instance.AddBananas(-GetUpgradeCost());
        Upgrade();
        return true;
    }
    
    public override int GetUpgradeCost()
    {
        if (level >= MAX_LEVEL) return 0;
        return upgradeCosts[level];
    }

    public override string GetUpgradeText()
    {   
        if(level < MAX_LEVEL) return $"Upgrade cost: {GetUpgradeCost()} bananas";
        else return "MAX LEVEL";
    }

    public void Upgrade()
    {   
        Assert.IsFalse(level >= MAX_LEVEL, "upgrading when we're already at or above max level!");
        ClearAllBuffs();
        
        level++;
        
        BuildingSoundManager.instance.PlayUpgradeSound();
        
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
        
        if (upgradeEffects != null)
        {
            upgradeEffects.Upgrade(level);
        }
        
        if (upgradeEffect != null) upgradeEffect.Play();
        
        UpdateRangeCircle();
        UpdateBuffedBuildings();
        
        if(level == MAX_LEVEL) canNeverBeUpgraded = true;
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