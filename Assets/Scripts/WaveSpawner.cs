using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float timeBetweenWaves = 30f;
    [SerializeField] private float baseEnemiesPerWave = 3f;
    [SerializeField] private float waveScalingFactor = 1.2f;
    private int bananaThreshold1 = 200;
    private int bananaThreshold2 = 500;
    private int bananaThreshold3 = 1000;
    private float bananaDifficultyMultiplier = 1.5f;
    
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool waveActive = false;

    void Start()
    {
        if (enemyPrefab == null)
        {
            return;
        }

        if (BuildingGrid.instance == null)
        {
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
            yield return new WaitForSeconds(timeBetweenWaves);
            
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
        if (!waveActive)
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
        int enemiesToSpawn = CalculateWaveSize();
        StartCoroutine(SpawnEnemies(enemiesToSpawn));
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
        Vector3 spawnPosition = GetRandomPerimeterPosition();
        
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        EnemyAttacker attacker = enemy.GetComponent<EnemyAttacker>();
        if (attacker != null)
        {
            enemiesAlive++;
            enemy.AddComponent<EnemyDeathTracker>().Initialize(this);
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

    public void OnEnemyDeath()
    {
        enemiesAlive--;
    }

    public int GetCurrentWave() => currentWave;
    public int GetEnemiesAlive() => enemiesAlive;
    public bool IsWaveActive() => waveActive;
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