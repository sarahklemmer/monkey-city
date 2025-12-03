using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    [Header("3D Leaf Animation")]
    [SerializeField] private Transform leftLeaf;
    [SerializeField] private Transform rightLeaf;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float leafSeparationDistance = 5f;
    [SerializeField] private float leafAnimationDuration = 1.5f;
    [SerializeField] private AnimationCurve leafCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private Vector3 leftLeafDirection = new Vector3(-1, 0, 0);
    [SerializeField] private Vector3 rightLeafDirection = new Vector3(1, 0, 0);
    
    [Header("Title")]
    [SerializeField] private CanvasGroup titleImage;
    [SerializeField] private CanvasGroup subtitleImage;
    [SerializeField] private float titleFadeDelay = 0f;
    [SerializeField] private float titleFadeDuration = 1f;
    
    [Header("UI Elements")]
    [SerializeField] private Button playButton;
    [SerializeField] private CanvasGroup playButtonGroup;
    [SerializeField] private float buttonFadeDelay = 1f;
    [SerializeField] private float buttonFadeDuration = 0.5f;
    
    [Header("Fake Gameplay Preview")]
    [SerializeField] private Transform previewCameraTarget;
    [SerializeField] private Transform treeOfLife;
    [SerializeField] private float previewRotationSpeed = 5f;
    [SerializeField] private bool enableCameraRotation = true;
    [SerializeField] private GameObject[] fakeGameplayObjects;
    [SerializeField] private bool autoCalculatePivot = false;
    
    [Header("Fake Gameplay Simulation")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject monkeyPrefab;
    [SerializeField] private float enemySpawnInterval = 15f;
    [SerializeField] private int enemiesPerWave = 2;
    [SerializeField] private bool enableFakeGameplay = true;
    
    [Header("Transition")]
    [SerializeField] private Image transitionOverlay;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Scene Loading")]
    [SerializeField] private string gameSceneName = "MainGame";
    
    private Vector3 leftLeafStartPos;
    private Vector3 rightLeafStartPos;
    private bool animationComplete = false;

    void Start()
    {
        DisableAllAudio();
        
        if (leftLeaf != null)
            leftLeafStartPos = leftLeaf.position;
        if (rightLeaf != null)
            rightLeafStartPos = rightLeaf.position;
        
        if (treeOfLife != null)
        {
            previewCameraTarget = treeOfLife;
        }
        else if (autoCalculatePivot && fakeGameplayObjects != null && fakeGameplayObjects.Length > 0)
        {
            Vector3 center = Vector3.zero;
            int count = 0;
            foreach (var obj in fakeGameplayObjects)
            {
                if (obj != null)
                {
                    center += obj.transform.position;
                    count++;
                }
            }
            if (count > 0)
            {
                center /= count;
                if (previewCameraTarget == null)
                {
                    GameObject pivotObj = new GameObject("AutoPivot");
                    previewCameraTarget = pivotObj.transform;
                }
                previewCameraTarget.position = center;
            }
        }
        
        if (transitionOverlay != null)
        {
            transitionOverlay.gameObject.SetActive(true);
            Color color = transitionOverlay.color;
            color.a = 1f;
            transitionOverlay.color = color;
        }
        
        if (titleImage != null)
            titleImage.alpha = 0;
        
        if (subtitleImage != null)
            subtitleImage.alpha = 0;
        
        if (playButtonGroup != null)
        {
            playButtonGroup.alpha = 0;
            playButtonGroup.interactable = false;
        }
        
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        
        StartCoroutine(PlayIntroSequence());
        
        if (enableFakeGameplay)
        {
            SetupFakeGameplay();
            StartCoroutine(SpawnFakeEnemies());
        }
    }

    private void DisableAllAudio()
    {
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.mute = true;
        }
        
        AudioListener.volume = 0f;
    }

    private IEnumerator PlayIntroSequence()
    {
        yield return StartCoroutine(FadeInFromBlack());
        
        yield return new WaitForSeconds(0.5f);
        
        if (enableCameraRotation && mainCamera != null)
        {
            StartCoroutine(RotateCamera());
        }
        
        if (fakeGameplayObjects != null && fakeGameplayObjects.Length > 0)
        {
            StartCoroutine(AnimateFakeGameplay());
        }
        
        yield return StartCoroutine(AnimateLeaves());
        
        yield return new WaitForSeconds(titleFadeDelay);
        yield return StartCoroutine(FadeInTitle());
        
        yield return new WaitForSeconds(buttonFadeDelay);
        yield return StartCoroutine(FadeInPlayButton());
        
        animationComplete = true;
    }

    private IEnumerator FadeInFromBlack()
    {
        if (transitionOverlay == null) yield break;
        
        float elapsed = 0f;
        
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            float curved = transitionCurve.Evaluate(t);
            
            Color color = transitionOverlay.color;
            color.a = Mathf.Lerp(1f, 0f, curved);
            transitionOverlay.color = color;
            
            yield return null;
        }
        
        Color finalColor = transitionOverlay.color;
        finalColor.a = 0f;
        transitionOverlay.color = finalColor;
    }

    private IEnumerator AnimateLeaves()
    {
        float elapsed = 0f;
        
        while (elapsed < leafAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / leafAnimationDuration;
            float curved = leafCurve.Evaluate(t);
            
            if (leftLeaf != null)
            {
                Vector3 leftTarget = leftLeafStartPos + leftLeafDirection.normalized * leafSeparationDistance;
                leftLeaf.position = Vector3.Lerp(leftLeafStartPos, leftTarget, curved);
            }
            
            if (rightLeaf != null)
            {
                Vector3 rightTarget = rightLeafStartPos + rightLeafDirection.normalized * leafSeparationDistance;
                rightLeaf.position = Vector3.Lerp(rightLeafStartPos, rightTarget, curved);
            }
            
            yield return null;
        }
        
        if (leftLeaf != null)
            Destroy(leftLeaf.gameObject);
        if (rightLeaf != null)
            Destroy(rightLeaf.gameObject);
    }

    private IEnumerator FadeInTitle()
    {
        if (titleImage == null)
        {
            yield break;
        }
        
        float elapsed = 0f;
        
        while (elapsed < titleFadeDuration)
        {
            elapsed += Time.deltaTime;
            titleImage.alpha = Mathf.Lerp(0, 1, elapsed / titleFadeDuration);
            subtitleImage.alpha = Mathf.Lerp(0, 1, elapsed / titleFadeDuration);
            yield return null;
        }
        
        titleImage.alpha = 1;
        subtitleImage.alpha = 1;
    }

    private IEnumerator FadeInPlayButton()
    {
        if (playButtonGroup == null)
        {
            Debug.LogError("Play Button Group is not assigned!");
            yield break;
        }
        
        float elapsed = 0f;
        
        while (elapsed < buttonFadeDuration)
        {
            elapsed += Time.deltaTime;
            playButtonGroup.alpha = Mathf.Lerp(0, 1, elapsed / buttonFadeDuration);
            yield return null;
        }
        
        playButtonGroup.alpha = 1;
        playButtonGroup.interactable = true;
    }

    private IEnumerator RotateCamera()
    {
        if (mainCamera == null) yield break;
        
        Vector3 pivot = previewCameraTarget != null ? previewCameraTarget.position : Vector3.zero;
        
        while (true)
        {
            float angle = previewRotationSpeed * Time.deltaTime;
            mainCamera.transform.RotateAround(pivot, Vector3.up, angle);
            mainCamera.transform.LookAt(pivot);
            
            yield return null;
        }
    }

    private IEnumerator AnimateFakeGameplay()
    {
        while (true)
        {
            foreach (var obj in fakeGameplayObjects)
            {
                if (obj != null)
                {
                    obj.transform.Rotate(Vector3.up, 20f * Time.deltaTime);
                }
            }
            yield return null;
        }
    }

    private void OnPlayButtonClicked()
    {
        if (!animationComplete) return;
        
        if (playButton != null)
            playButton.interactable = false;
        
        StartCoroutine(FadeTransitionToGame());
    }

    private IEnumerator FadeTransitionToGame()
    {
        
        if (transitionOverlay == null)
        {
            Debug.LogError("Transition Overlay is not assigned!");
            yield break;
        }
        
        if (titleImage != null)
        {
            StartCoroutine(FadeOutUI(titleImage, transitionDuration * 0.5f));
        }
        if (playButtonGroup != null)
        {
            StartCoroutine(FadeOutUI(playButtonGroup, transitionDuration * 0.5f));
        }
        
        yield return new WaitForSeconds(transitionDuration * 0.3f);
        
        transitionOverlay.gameObject.SetActive(true);
        transitionOverlay.enabled = true;
        Color startColor = transitionOverlay.color;
        startColor.a = 0;
        transitionOverlay.color = startColor;
        
        
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            float curved = transitionCurve.Evaluate(t);
            
            Color color = transitionOverlay.color;
            color.a = Mathf.Lerp(0f, 1f, curved);
            transitionOverlay.color = color;
            
            yield return null;
        }
        
        Color finalColor = transitionOverlay.color;
        finalColor.a = 1f;
        transitionOverlay.color = finalColor;
        
        yield return new WaitForSeconds(0.2f);
        
        AudioListener.volume = 1f;
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }
    
    private IEnumerator FadeOutUI(CanvasGroup group, float duration)
    {
        float elapsed = 0f;
        float startAlpha = group.alpha;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, 0, elapsed / duration);
            yield return null;
        }
        
        group.alpha = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && animationComplete)
        {
            OnPlayButtonClicked();
        }
    }

    private void SetupFakeGameplay()
    {
        if (monkeyPrefab == null)
        {
            Debug.LogWarning("Monkey prefab not assigned! Towers won't shoot.");
            return;
        }
        
        BananaFarm[] farms = FindObjectsByType<BananaFarm>(FindObjectsSortMode.None);
        foreach (var farm in farms)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 spawnPos = farm.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                GameObject monkeyObj = Instantiate(monkeyPrefab, spawnPos, Quaternion.identity);
                MonkeyController monkey = monkeyObj.GetComponent<MonkeyController>();
                if (monkey != null)
                {
                    MonkeyAlloc alloc = monkey.GetComponent<MonkeyAlloc>();
                    if (alloc != null)
                    {
                        alloc.Assign(farm);
                    }
                }
            }
        }
        
        ArcherTower[] towers = FindObjectsByType<ArcherTower>(FindObjectsSortMode.None);
        foreach (var tower in towers)
        {
            Vector3 spawnPos = tower.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            GameObject monkeyObj = Instantiate(monkeyPrefab, spawnPos, Quaternion.identity);
            MonkeyController monkey = monkeyObj.GetComponent<MonkeyController>();
            if (monkey != null)
            {
                MonkeyAlloc alloc = monkey.GetComponent<MonkeyAlloc>();
                if (alloc != null)
                {
                    alloc.Assign(tower);
                }
            }
        }
    }

    private IEnumerator SpawnFakeEnemies()
    {
        yield return new WaitForSeconds(5f);
        
        while (true)
        {
            if (enemyPrefab != null && BuildingGrid.instance != null)
            {
                for (int i = 0; i < enemiesPerWave; i++)
                {
                    Vector3 spawnPos = GetRandomPerimeterPosition();
                    GameObject enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

                    yield return new WaitForSeconds(3f); 
                }
            }
            
            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }

    private Vector3 GetRandomPerimeterPosition()
    {
        int gridSize = BuildingGrid.instance != null ? BuildingGrid.instance.GetGridSize() : 20;
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
}

public class TitleScreenInvincible : MonoBehaviour
{
    private BuildingHealth buildingHealth;
    private float minHealth = 1f;
    
    public void Initialize(BuildingHealth health)
    {
        buildingHealth = health;
    }
    
    void LateUpdate()
    {
        if (buildingHealth != null)
        {
            if (buildingHealth.GetCurrentHealth() < minHealth)
            {
                float maxHealth = buildingHealth.GetMaxHealth();
                buildingHealth.SetHealth(maxHealth * 0.5f);
            }
        }
    }
}