using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Buildings.Building_Controllers;

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
    [SerializeField] private int bananasRequiredToStartAttacks = 20;
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
    private int lastWarningDay = -999;
    private bool firstTowerPlaced = false;
    private bool firstWaveTriggered = false;
    private ArcherTower firstArcherTower = null;

    [SerializeField] private int currentBananas = 0;
    private List<Library> registeredLibraries = new List<Library>();

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
        Debug.Log("WaveSpawner: Instance created and ready");
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
        
        // DEBUG - Press 2 to see wave status
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            int bananas = BananaManager.instance.GetBananasGenerated();
            int currentDay = TimeController.instance.currentDay;
            int daysSinceLastWave = currentDay - lastWaveDay;
            int daysRequired = GetDaysBetweenWaves();
            Debug.Log($"=== WAVE DEBUG ===");
            Debug.Log($"Current Wave: {currentWave} | Enemies Alive: {enemiesAlive}");
            Debug.Log($"Attacks Unlocked: {attacksUnlocked} | Wave Active: {waveActive}");
            Debug.Log($"Bananas: {bananas} | Tutorial Done: {firstWaveTriggered}");
            Debug.Log($"Current Day: {currentDay} | Last Wave Day: {lastWaveDay}");
            Debug.Log($"Days Since Last Wave: {daysSinceLastWave}/{daysRequired}");
        }
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            while (spawningPaused)
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            // FIRST WAVE - Tutorial wave (triggered by tower placement OR 20 bananas)
            if (currentWave == 0 && !firstWaveTriggered)
            {
                currentBananas = BananaManager.instance.GetBananasGenerated();
                
                // Trigger tutorial if EITHER condition is met
                if (firstTowerPlaced || currentBananas >= bananasRequiredToStartAttacks)
                {
                    if (firstTowerPlaced)
                        toastManager.RequestToast($"Man the archer tower, an enemy is attacking!", 2.0f, 0.3f, false, false);
                    else
                        toastManager.RequestToast($"Enemy spotted! Defend yourself!", 2.0f, 0.3f, false, false);
                    
                    firstWaveTriggered = true;
                    currentWave++;
                    lastWaveDay = TimeController.instance.currentDay;
                    
                    yield return new WaitForSeconds(2f);
                    StartFirstWave();
                    
                    while (enemiesAlive > 0)
                    {
                        yield return new WaitForSeconds(0.5f);
                    }

                    if (enemiesAlive == 0)
                    {
                        int points = GetPointsForWave();
                        PathManager.instance.playerPoints += points;
                        Debug.Log($"Wave {currentWave} complete! Awarded {points} points. Library count: {registeredLibraries.Count}");
                        
                        // Give grace period before wave 2
                        yield return new WaitForSeconds(3f);
                    }
                    
                    waveActive = false;
                    BuildingManager.instance.HealAllToFull();
                    ChoosePathSystem.instance.ShowPathMenu();
                    
                    // Make wave 2 spawn in 3 days
                    attacksUnlocked = true;
                    lastWaveDay = TimeController.instance.currentDay - (daysBetweenWaves - 3); // Wave 2 spawns in 3 days
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
            }
            
            // Regular waves system (already unlocked after tutorial)
            if (!attacksUnlocked)
            {
                // This should never happen now since tutorial unlocks it
                yield return new WaitForSeconds(1f);
                continue;
            }
            
            int bananas = BananaManager.instance.GetBananasGenerated();
            int currentThreshold = GetCurrentThreshold(bananas);
            
            if (currentThreshold > lastBananaThresholdCrossed)
            {
                lastBananaThresholdCrossed = currentThreshold;
                lastWaveDay = TimeController.instance.currentDay;
                yield return new WaitForSeconds(gracePeriodDays * 30f);
            }
            
            int currentDay = TimeController.instance.currentDay;
            int daysRequired = GetDaysBetweenWaves();
            int daysSinceLastWave = currentDay - lastWaveDay;
            
            // Wave warning
            if (daysSinceLastWave == daysRequired - 1 && lastWarningDay != currentDay)
            {
                lastWarningDay = currentDay; 
                bool isBossWave = ((currentWave + 1) % 7 == 0) && bossPrefab != null;
                if (isBossWave)
                {
                    toastManager.RequestToast($"BOSS WAVE Incoming Tomorrow! Prepare Yourself!", 3.0f, 0.3f, false, false);
                }
                else
                {
                    toastManager.RequestToast($"Wave {currentWave + 1} Incoming Tomorrow! Prepare Your Defenses!", 3.0f, 0.3f, false, false);
                }
            }
            
            // Start wave
            if (daysSinceLastWave >= daysRequired)
            {
                currentWave++;
                lastWaveDay = currentDay;
                
                int enemiesToSpawn = CalculateWaveSize();
                bool isBossWave = (currentWave % 7 == 0) && bossPrefab != null;
                
                if (isBossWave)
                    toastManager.RequestToast($"BOSS WAVE {currentWave} Starting NOW!", 2.0f, 0.3f, false, false);
                else
                    toastManager.RequestToast($"Enemy Wave {currentWave} Incoming!", 2.0f, 0.3f, false, false);
                
                yield return new WaitForSeconds(2f);
                StartWave(enemiesToSpawn);
                
                while (enemiesAlive > 0)
                {
                    yield return new WaitForSeconds(0.5f);
                }
                
                if (enemiesAlive == 0)
                {
                    int points = GetPointsForWave();
                    PathManager.instance.playerPoints += points;
                    Debug.Log($"Wave {currentWave} complete! Awarded {points} points.");
                }
                
                waveActive = false;
                BuildingManager.instance.HealAllToFull();

                if (currentWave == 1)
                {
                    ChoosePathSystem.instance.ShowPathMenu();
                }
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
            StartCoroutine(AnnounceAndStartWave(enemiesToSpawn, (currentWave % 7 == 0) && bossPrefab != null));
            StartCoroutine(WaveLoop());
        }
    }

    private void StartFirstWave()
    {
        waveActive = true;
        
        if (firstArcherTower != null)
        {
            Vector3 spawnPosition = GetNearestPerimeterPosition(firstArcherTower.transform.position);
            SpawnEnemyAt(spawnPosition, true); // Force base chimp for tutorial
        }
        else
        {
            Vector3 spawnPosition = GetRandomPerimeterPosition();
            SpawnEnemyAt(spawnPosition, true);
        }
    }

    private void StartWave(int enemiesToSpawn)
    {
        waveActive = true;
        
        bool isBossWave = (currentWave % 7 == 0) && bossPrefab != null;
        
        if (isBossWave)
        {
            SpawnBoss();
        }
        else
        {
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
        
        return Mathf.CeilToInt(waveSize);
    }

    IEnumerator AnnounceAndStartWave(int enemiestoSpawn, bool isBossWave)
    {
        if (isBossWave)
            toastManager.RequestToast($"BOSS WAVE Incoming! Prepare Yourself!", 2.0f, 0.3f, false, false);
        else
            toastManager.RequestToast($"Enemy Wave {currentWave} Incoming!", 2.0f, 0.3f, false, false);
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
        Vector3 spawnPosition = GetRandomPerimeterPosition();
        SpawnEnemyAt(spawnPosition, false);
    }

    private void SpawnEnemyAt(Vector3 spawnPosition, bool forceBaseEnemy = false)
    {
        int bananas = BananaManager.instance.GetBananasGenerated();
        GameObject selectedPrefab;
        
        // For tutorial wave, always spawn base enemy
        if (forceBaseEnemy)
        {
            selectedPrefab = enemyTypes[0].enemyPrefab; // First enemy is base chimp
        }
        else
        {
            // Get available enemies based on banana threshold
            var availableEnemies = enemyTypes.Where(e => e.bananaThreshold <= bananas).ToList();
            
            if (availableEnemies.Count == 0)
            {
                Debug.LogWarning("No enemies available for current banana count!");
                return;
            }
            
            // Weighted random selection
            float totalWeight = availableEnemies.Sum(e => e.spawnWeight);
            float randomValue = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            
            selectedPrefab = availableEnemies[0].enemyPrefab;
            foreach (var enemyConfig in availableEnemies)
            {
                cumulative += enemyConfig.spawnWeight;
                if (randomValue <= cumulative)
                {
                    selectedPrefab = enemyConfig.enemyPrefab;
                    Debug.Log($"Spawning {enemyConfig.enemyName} (threshold: {enemyConfig.bananaThreshold}, weight: {enemyConfig.spawnWeight})");
                    break;
                }
            }
        }
        
        GameObject spawnedEnemy = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        
        EnemyAttacker attacker = spawnedEnemy.GetComponent<EnemyAttacker>();
        if (attacker != null)
        {
            enemiesAlive++;
            spawnedEnemy.AddComponent<EnemyDeathTracker>().Initialize(this);
            
            // Tutorial wave targeting
            if (currentWave == 1 && firstArcherTower != null)
            {
                attacker.SetPriorityTargetToFirstTower();
            }
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
        
        Debug.Log("BOSS SPAWNED!");
    }

    private Vector3 GetNearestPerimeterPosition(Vector3 targetPosition)
    {
        int gridSize = BuildingGrid.instance.GetGridSize();
        float halfSize = gridSize / 2f;
        float spawnDistance = 1.3f;
        
        Vector3[] perimeterPositions = new Vector3[]
        {
            new Vector3(0, 0, halfSize * spawnDistance),
            new Vector3(halfSize * spawnDistance, 0, 0),
            new Vector3(0, 0, -halfSize * spawnDistance),
            new Vector3(-halfSize * spawnDistance, 0, 0)
        };
        
        Vector3 nearestPosition = perimeterPositions[0];
        float nearestDistance = Vector3.Distance(targetPosition, nearestPosition);
        
        for (int i = 1; i < perimeterPositions.Length; i++)
        {
            float distance = Vector3.Distance(targetPosition, perimeterPositions[i]);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPosition = perimeterPositions[i];
            }
        }
        
        return nearestPosition;
    }

    private Vector3 GetRandomPerimeterPosition()
    {
        int gridSize = BuildingGrid.instance.GetGridSize();
        float halfSize = gridSize / 2f;
        float spawnDistance = 1.3f;
    
        int edge = Random.Range(0, 4);
    
        float x, z;
    
        switch (edge)
        {
            case 0:
                x = Random.Range(-halfSize, halfSize);
                z = halfSize * spawnDistance;
                break;
            case 1:
                x = halfSize * spawnDistance;
                z = Random.Range(-halfSize, halfSize);
                break;
            case 2:
                x = Random.Range(-halfSize, halfSize);
                z = -halfSize * spawnDistance;
                break;
            case 3:
                x = -halfSize * spawnDistance;
                z = Random.Range(-halfSize, halfSize);
                break;
            default:
                x = 0;
                z = halfSize * spawnDistance;
                break;
        }
    
        return new Vector3(x, 0, z);
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
    
    // Library Support Methods
    public void RegisterLibrary(Library library)
    {
        if (!registeredLibraries.Contains(library))
        {
            registeredLibraries.Add(library);
            Debug.Log($"WaveSpawner: Library registered! Point bonus now active. Total libraries: {registeredLibraries.Count}");
        }
    }
    
    public void UnregisterLibrary(Library library)
    {
        if (registeredLibraries.Contains(library))
        {
            registeredLibraries.Remove(library);
            Debug.Log($"WaveSpawner: Library unregistered. Point bonus removed. Total libraries: {registeredLibraries.Count}");
        }
    }
    
    public int GetRegisteredLibraryCount() => registeredLibraries.Count;
    
    private int GetPointsForWave()
    {
        int basePoints = 1;
        
        if (registeredLibraries.Count > 0)
        {
            Debug.Log($"WaveSpawner: Library bonus applied! {basePoints} x 2 = {basePoints * 2}");
            return basePoints * 2;
        }
        
        return basePoints;
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
    
    public void OnFirstTowerPlaced(ArcherTower tower)
    {
        if (!firstTowerPlaced)
        {
            firstTowerPlaced = true;
            firstArcherTower = tower;
        }
    }
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