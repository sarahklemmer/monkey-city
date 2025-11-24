using UnityEngine;
using System.Collections;
using System.Linq;

[System.Serializable]
public class EnemyWaveConfig
{
    public GameObject enemyPrefab;
    public int bananaThreshold;
    public float spawnWeight = 1f;
    public string enemyName = "Enemy";
}

public class WaveSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private int daysBetweenWaves = 4;
    [SerializeField] private float baseEnemiesPerWave = 2f;
    [SerializeField] private float waveScalingFactor = 1.15f;
    
    [Header("Enemy Types")]
    [SerializeField] private EnemyWaveConfig[] enemyTypes;
    [SerializeField] private GameObject bossPrefab;
    
    [Header("Difficulty Scaling")]
    [SerializeField] private int bananasRequiredToStartAttacks = 100;
    private int bananaThreshold1 = 300;
    private int bananaThreshold2 = 800;
    private int bananaThreshold3 = 1500;
    private float bananaDifficultyMultiplier = 1.3f;
    
    [Header("Wave Timing Adjustments")]
    [SerializeField] private int dayReductionPerThreshold = 1;
    [SerializeField] private int minimumDaysBetweenWaves = 2;
    [SerializeField] private int gracePeriodDays = 2;
    
    private ToastManager toastManager;
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool waveActive = false;
    private bool attacksUnlocked = false;
    private bool spawningPaused = false;
    private int lastBananaThresholdCrossed = 0;
    private int lastWaveDay = 0;
    private int lastWarningDay = -999; // Track which day we showed warning

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
        // Cheat: Press 1 to force spawn next wave
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ForceNextWave();
            Debug.Log("Cheat activated: Wave spawned!");
        }
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            while (spawningPaused)
            {
                Debug.Log("spawning paused");
                yield return new WaitForSeconds(0.5f);
            }
            
            if (!attacksUnlocked)
            {
                int currentBananas = BananaManager.instance.GetBananasGenerated();
                if (currentBananas >= bananasRequiredToStartAttacks)
                {
                    attacksUnlocked = true;
                    lastWaveDay = TimeController.instance.currentDay;
                    Debug.Log("⚠️ Your banana wealth has attracted attention! Enemy waves incoming!");
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }
            }
            
            int bananas = BananaManager.instance.GetBananasGenerated();
            int currentThreshold = GetCurrentThreshold(bananas);
            
            if (currentThreshold > lastBananaThresholdCrossed)
            {
                lastBananaThresholdCrossed = currentThreshold;
                Debug.Log($"⚠️ Difficulty tier {currentThreshold} reached! Stronger enemies and faster waves incoming!");
                lastWaveDay = TimeController.instance.currentDay;
                yield return new WaitForSeconds(gracePeriodDays * 30f);
            }
            
            int currentDay = TimeController.instance.currentDay;
            int daysRequired = GetDaysBetweenWaves();
            int daysSinceLastWave = currentDay - lastWaveDay;
            
            // Show warning 1 day before wave (only once per day)
            if (daysSinceLastWave == daysRequired - 1 && lastWarningDay != currentDay)
            {
                lastWarningDay = currentDay; // Mark this day as having shown the warning
                bool isBossWave = ((currentWave + 1) % 7 == 0) && bossPrefab != null;
                if (isBossWave)
                {
                    toastManager.RequestToast($"🚨 BOSS WAVE Incoming Tomorrow! Prepare Yourself! 🚨", 3.0f, 0.3f, false, false);
                    Debug.Log($"Warning: Boss wave {currentWave + 1} incoming tomorrow!");
                }
                else
                {
                    toastManager.RequestToast($"⚠️ Wave {currentWave + 1} Incoming Tomorrow! Prepare Your Defenses!", 3.0f, 0.3f, false, false);
                    Debug.Log($"Warning: Wave {currentWave + 1} incoming tomorrow!");
                }
            }
            
            if (daysSinceLastWave >= daysRequired)
            {
                currentWave++;
                PathManager.instance.playerPoints += 1;
                lastWaveDay = currentDay;
                
                int enemiesToSpawn = CalculateWaveSize();
                bool isBossWave = (currentWave % 7 == 0) && bossPrefab != null;
                
                if (isBossWave)
                    toastManager.RequestToast($"🚨 BOSS WAVE {currentWave} Starting NOW! 🚨", 2.0f, 0.3f, false, false);
                else
                    toastManager.RequestToast($"Chimpanzees Incoming! Wave {currentWave} Starting!", 2.0f, 0.3f, false, false);
                
                yield return new WaitForSeconds(2f);
                StartWave(enemiesToSpawn);
                
                while (enemiesAlive > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                }
                
                waveActive = false;
                HealAllBuildings();

            PathManager.instance.playerPoints += 1;

            if (currentWave == 1)
            {
                ChoosePathSystem.instance.ShowPathMenu();
            }
            
            yield return new WaitForSeconds(1f);
        }
    }

    private void ForceNextWave()
    {
        if (!waveActive && attacksUnlocked)
        {
            StopAllCoroutines();
            currentWave++;
            lastWaveDay = TimeController.instance.currentDay;
            int enemiesToSpawn = CalculateWaveSize();
            StartCoroutine(AnnounceAndStartWave(enemiesToSpawn, (currentWave % 5 == 0) && bossPrefab != null));
            StartCoroutine(WaveLoop());
        }
    }

    private void StartWave(int enemiesToSpawn)
    {
        waveActive = true;
        
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
            toastManager.RequestToast($"Chimpanzees Incoming! Wave {currentWave}", 2.0f, 0.3f, false, false);
        yield return new WaitForSeconds(2f);
        StartWave(enemiestoSpawn);
    }

    private IEnumerator SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.8f);
        }
    }

    private void SpawnEnemy()
    {
        int bananas = BananaManager.instance.GetBananasGenerated();
        var availableEnemies = enemyTypes.Where(e => e.bananaThreshold <= bananas).ToList();
        
        if (availableEnemies.Count == 0)
        {
            Debug.LogWarning("No available enemy types for current banana count!");
            return;
        }
        
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

    private int GetDaysBetweenWaves()
    {
        int bananas = BananaManager.instance.GetBananasGenerated();
        int dayReduction = 0;
        
        if (bananas >= bananaThreshold3)
            dayReduction = dayReductionPerThreshold * 3;
        else if (bananas >= bananaThreshold2)
            dayReduction = dayReductionPerThreshold * 2;
        else if (bananas >= bananaThreshold1)
            dayReduction = dayReductionPerThreshold;
        
        return Mathf.Max(minimumDaysBetweenWaves, daysBetweenWaves - dayReduction);
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
    public bool IsPaused() => spawningPaused;
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