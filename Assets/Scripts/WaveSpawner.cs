using UnityEngine;
using System.Collections;
using System.Linq;

[System.Serializable]
public class EnemyWaveConfig
{
    public GameObject enemyPrefab;
    public int bananaThreshold;
    public float spawnWeight = 1f; // Higher = more likely to spawn
    public string enemyName = "Enemy";
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float timeBetweenWaves = 30f;
    [SerializeField] private float baseEnemiesPerWave = 3f;
    [SerializeField] private float waveScalingFactor = 1.2f;
    
    [Header("Enemy Types")]
    [SerializeField] private EnemyWaveConfig[] enemyTypes;
    [SerializeField] private GameObject bossPrefab;
    
    [Header("Difficulty Scaling")]
    [SerializeField] private int bananasRequiredToStartAttacks = 50;
    private int bananaThreshold1 = 200;
    private int bananaThreshold2 = 500;
    private int bananaThreshold3 = 1000;
    private float bananaDifficultyMultiplier = 1.5f;
    
    [Header("Wave Timing Adjustments")]
    [SerializeField] private float timeReductionPerThreshold = 5f;
    [SerializeField] private float minimumTimeBetweenWaves = 10f;
    [SerializeField] private float gracePeriodAfterThreshold = 10f;
    
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool waveActive = false;
    private bool attacksUnlocked = false;
    private int lastBananaThresholdCrossed = 0;

    void Start()
    {
        if (enemyTypes == null || enemyTypes.Length == 0)
        {
            Debug.LogError("No enemy types assigned to WaveSpawner!");
            return;
        }

        if (BuildingGrid.instance == null)
        {
            Debug.LogError("BuildingGrid instance not found!");
            return;
        }

        StartCoroutine(WaveLoop());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ForceNextWave();
        }
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            // Check if attacks should start
            if (!attacksUnlocked)
            {
                int currentBananas = BananaManager.instance.GetBananas();
                if (currentBananas >= bananasRequiredToStartAttacks)
                {
                    attacksUnlocked = true;
                    Debug.Log("⚠️ Your banana wealth has attracted attention! Enemy waves incoming!");
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }
            }
            
            // Check if player crossed a new threshold
            int bananas = BananaManager.instance.GetBananas();
            int currentThreshold = GetCurrentThreshold(bananas);
            
            if (currentThreshold > lastBananaThresholdCrossed)
            {
                lastBananaThresholdCrossed = currentThreshold;
                Debug.Log($"⚠️ Difficulty tier {currentThreshold} reached! Stronger enemies and faster waves incoming!");
                yield return new WaitForSeconds(gracePeriodAfterThreshold);
            }
            
            yield return new WaitForSeconds(GetTimeBetweenWaves());
            
            currentWave++;
            StartWave();
            
            while (enemiesAlive > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            waveActive = false;
        }
    }

    private void ForceNextWave()
    {
        if (!waveActive && attacksUnlocked)
        {
            StopAllCoroutines();
            currentWave++;
            StartWave();
            StartCoroutine(WaveLoop());
        }
    }

    private void StartWave()
    {
        waveActive = true;
        
        // Check if this is a boss wave (every 5 waves)
        bool isBossWave = (currentWave % 5 == 0) && bossPrefab != null;
        
        if (isBossWave)
        {
            Debug.Log($"🚨 BOSS WAVE {currentWave}! 🚨");
            SpawnBoss();
        }
        else
        {
            Debug.Log($"Wave {currentWave} starting...");
            int enemiesToSpawn = CalculateWaveSize();
            StartCoroutine(SpawnEnemies(enemiesToSpawn));
        }
    }

    private int CalculateWaveSize()
    {
        float waveSize = baseEnemiesPerWave * Mathf.Pow(waveScalingFactor, currentWave - 1);
        
        int bananas = BananaManager.instance.GetBananas();
        float bananaMultiplier = 1f;
        
        if (bananas >= bananaThreshold3)
        {
            bananaMultiplier = Mathf.Pow(bananaDifficultyMultiplier, 3);
        }
        else if (bananas >= bananaThreshold2)
        {
            bananaMultiplier = Mathf.Pow(bananaDifficultyMultiplier, 2);
        }
        else if (bananas >= bananaThreshold1)
        {
            bananaMultiplier = bananaDifficultyMultiplier;
        }
        
        waveSize *= bananaMultiplier;
        
        Debug.Log($"Wave size: Base={baseEnemiesPerWave * Mathf.Pow(waveScalingFactor, currentWave - 1):F1}, Banana multiplier={bananaMultiplier:F1}, Final={Mathf.CeilToInt(waveSize)}");
        
        return Mathf.CeilToInt(waveSize);
    }

    private IEnumerator SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SpawnEnemy()
    {
        // Get available enemy types based on banana count
        int bananas = BananaManager.instance.GetBananas();
        var availableEnemies = enemyTypes.Where(e => e.bananaThreshold <= bananas).ToList();
        
        if (availableEnemies.Count == 0)
        {
            Debug.LogWarning("No available enemy types for current banana count!");
            return;
        }
        
        // Weighted random selection
        float totalWeight = availableEnemies.Sum(e => e.spawnWeight);
        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        
        GameObject selectedPrefab = availableEnemies[0].enemyPrefab;
        foreach (var enemyConfig in availableEnemies)
        {
            cumulative += enemyConfig.spawnWeight;
            if (randomValue <= cumulative)
            {
                selectedPrefab = enemyConfig.enemyPrefab;
                break;
            }
        }
        
        Vector3 spawnPosition = GetRandomPerimeterPosition();
        GameObject spawnedEnemy = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        
        EnemyAttacker attacker = spawnedEnemy.GetComponent<EnemyAttacker>();
        if (attacker != null)
        {
            enemiesAlive++;
            spawnedEnemy.AddComponent<EnemyDeathTracker>().Initialize(this);
        }
    }

    private void SpawnBoss()
    {
        Vector3 spawnPosition = GetRandomPerimeterPosition();
        GameObject boss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        
        EnemyAttacker attacker = boss.GetComponent<EnemyAttacker>();
        if (attacker != null)
        {
            enemiesAlive++;
            boss.AddComponent<EnemyDeathTracker>().Initialize(this);
        }
    }

    private Vector3 GetRandomPerimeterPosition()
    {
        int gridSize = BuildingGrid.instance.GetGridSize();
        float halfSize = gridSize / 2f;
        
        int edge = Random.Range(0, 4);
        
        float x, z;
        
        switch (edge)
        {
            case 0:
                x = Random.Range(-halfSize, halfSize);
                z = halfSize;
                break;
            case 1:
                x = halfSize;
                z = Random.Range(-halfSize, halfSize);
                break;
            case 2:
                x = Random.Range(-halfSize, halfSize);
                z = -halfSize;
                break;
            case 3:
                x = -halfSize;
                z = Random.Range(-halfSize, halfSize);
                break;
            default:
                x = 0;
                z = halfSize;
                break;
        }
        
        return new Vector3(x, 0, z);
    }

    private float GetTimeBetweenWaves()
    {
        int bananas = BananaManager.instance.GetBananas();
        float timeReduction = 0f;
        
        if (bananas >= bananaThreshold3)
            timeReduction = timeReductionPerThreshold * 3;
        else if (bananas >= bananaThreshold2)
            timeReduction = timeReductionPerThreshold * 2;
        else if (bananas >= bananaThreshold1)
            timeReduction = timeReductionPerThreshold;
        
        return Mathf.Max(minimumTimeBetweenWaves, timeBetweenWaves - timeReduction);
    }

    private int GetCurrentThreshold(int bananas)
    {
        if (bananas >= bananaThreshold3) return 3;
        if (bananas >= bananaThreshold2) return 2;
        if (bananas >= bananaThreshold1) return 1;
        return 0;
    }

    public void OnEnemyDeath()
    {
        enemiesAlive--;
    }

    public int GetCurrentWave() => currentWave;
    public int GetEnemiesAlive() => enemiesAlive;
    public bool IsWaveActive() => waveActive;
    public bool AreAttacksUnlocked() => attacksUnlocked;
}

public class EnemyDeathTracker : MonoBehaviour
{
    private WaveSpawner spawner;
    
    public void Initialize(WaveSpawner waveSpawner)
    {
        spawner = waveSpawner;
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnEnemyDeath();
        }
    }
}