using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor.PackageManager.Requests;

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
    [SerializeField] private float timeBetweenWaves = 45f; // Increased from 30s
    [SerializeField] private float baseEnemiesPerWave = 2f; // Reduced from 3
    [SerializeField] private float waveScalingFactor = 1.15f; // Reduced from 1.2
    
    [Header("Enemy Types")]
    [SerializeField] private EnemyWaveConfig[] enemyTypes;
    [SerializeField] private GameObject bossPrefab;
    
    [Header("Difficulty Scaling")]
    [SerializeField] private int bananasRequiredToStartAttacks = 100; // Increased from 50
    private int bananaThreshold1 = 300; // Increased from 200
    private int bananaThreshold2 = 800; // Increased from 500
    private int bananaThreshold3 = 1500; // Increased from 1000
    private float bananaDifficultyMultiplier = 1.3f; // Reduced from 1.5
    
    [Header("Wave Timing Adjustments")]
    [SerializeField] private float timeReductionPerThreshold = 3f; // Reduced from 5f
    [SerializeField] private float minimumTimeBetweenWaves = 20f; // Increased from 10f
    [SerializeField] private float gracePeriodAfterThreshold = 15f; // Increased from 10f
    
    private ToastManager toastManager;
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool waveActive = false;
    private bool attacksUnlocked = false;
    private bool spawningPaused = false;
    private int lastBananaThresholdCrossed = 0;

    public static WaveSpawner instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate WaveSpawner on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

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
        toastManager = ToastManager.Instance;

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
            if (spawningPaused)
            {
                Debug.Log("spawning paused");
                yield return new WaitForSeconds(0.5f);
            }
            // Check if attacks should start
            if (!attacksUnlocked)
            {
                int currentBananas = BananaManager.instance.GetBananasGenerated();
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
            int bananas = BananaManager.instance.GetBananasGenerated();
            int currentThreshold = GetCurrentThreshold(bananas);
            
            if (currentThreshold > lastBananaThresholdCrossed)
            {
                lastBananaThresholdCrossed = currentThreshold;
                Debug.Log($"⚠️ Difficulty tier {currentThreshold} reached! Stronger enemies and faster waves incoming!");
                yield return new WaitForSeconds(gracePeriodAfterThreshold);
            }
            
            yield return new WaitForSeconds(GetTimeBetweenWaves());
            
            currentWave++;
            int enemiesToSpawn = CalculateWaveSize();
            StartCoroutine(AnnounceAndStartWave(enemiesToSpawn, (currentWave % 5 == 0) && bossPrefab != null));
            yield return new WaitForSeconds(2f);
            while (enemiesAlive > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            waveActive = false;
            
            // Heal all buildings to full health after wave ends
            HealAllBuildings();
        }
    }

    private void ForceNextWave()
    {
        if (!waveActive && attacksUnlocked)
        {
            StopAllCoroutines();
            currentWave++;
            int enemiesToSpawn = CalculateWaveSize();
            StartCoroutine(AnnounceAndStartWave(enemiesToSpawn, (currentWave % 5 == 0) && bossPrefab != null));
            StartCoroutine(WaveLoop());
        }
    }

    private void StartWave(int enemiesToSpawn)
    {
        waveActive = true;
        
        // Check if this is a boss wave (every 7 waves instead of 5)
        bool isBossWave = (currentWave % 7 == 0) && bossPrefab != null;
        
        if (isBossWave)
        {
            Debug.Log($"🚨 BOSS WAVE {currentWave}! 🚨");
            SpawnBoss();
        }
        else
        {
            Debug.Log($"Wave {currentWave} starting...");
            
            StartCoroutine(SpawnEnemies(enemiesToSpawn));
        }
    }

    private int CalculateWaveSize()
    {
        float waveSize = baseEnemiesPerWave * Mathf.Pow(waveScalingFactor, currentWave - 1);
        
        int bananas = BananaManager.instance.GetBananasGenerated();
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

    IEnumerator AnnounceAndStartWave(int enemiestoSpawn, bool isBossWave)
    {
        if (isBossWave)
            toastManager.RequestToast($"🚨 BOSS WAVE Incoming! Prepare Yourself! 🚨", 2.0f, 0.3f, false, false);
        else
            toastManager.RequestToast($"Chimpanzees Incoming! Wave Size: " + enemiestoSpawn, 2.0f, 0.3f, false, false);
        yield return new WaitForSeconds(2f);
        StartWave(enemiestoSpawn);
    }

    private IEnumerator SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.8f); // Increased from 0.5s - more time between spawns
        }
    }

    private void SpawnEnemy()
    {
        // Get available enemy types based on banana count
        int bananas = BananaManager.instance.GetBananasGenerated();
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

    private void HealAllBuildings()
    {
        BuildingHealth[] allBuildings = FindObjectsByType<BuildingHealth>(FindObjectsSortMode.None);
        
        int healedCount = 0;
        foreach (BuildingHealth building in allBuildings)
        {
            if (building != null && building.GetCurrentHealth() > 0)
            {
                building.HealToFull();
                healedCount++;
            }
        }
        
        if (healedCount > 0)
        {
            Debug.Log($"✨ Wave cleared! All {healedCount} buildings restored to full health!");
        }
    }

    private float GetTimeBetweenWaves()
    {
        int bananas = BananaManager.instance.GetBananasGenerated();
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
    public void PauseSpawning() { spawningPaused = true; }
    public void UnpauseSpawning() { spawningPaused = false; }
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