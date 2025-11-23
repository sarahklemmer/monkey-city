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
    [SerializeField] private CanvasGroup titleText;
    [SerializeField] private float titleFadeDelay = 0.5f;
    [SerializeField] private float titleFadeDuration = 1f;
    
    [Header("UI Elements")]
    [SerializeField] private Button playButton;
    [SerializeField] private CanvasGroup playButtonGroup;
    [SerializeField] private float buttonFadeDelay = 1f;
    [SerializeField] private float buttonFadeDuration = 0.5f;
    
    [Header("Gameplay Preview")]
    [SerializeField] private Camera gameplayPreviewCamera;
    [SerializeField] private Transform previewCameraTarget;
    [SerializeField] private float previewRotationSpeed = 5f;
    [SerializeField] private bool enablePreview = true;
    
    [Header("Scene Loading")]
    [SerializeField] private string gameSceneName = "MainGame";
    [SerializeField] private float sceneTransitionDelay = 0.3f;
    
    private Vector3 leftLeafStartPos;
    private Vector3 rightLeafStartPos;
    private bool animationComplete = false;

    void Start()
    {
        if (leftLeaf != null)
            leftLeafStartPos = leftLeaf.position;
        if (rightLeaf != null)
            rightLeafStartPos = rightLeaf.position;
        
        if (titleText != null)
            titleText.alpha = 0;
        
        if (playButtonGroup != null)
        {
            playButtonGroup.alpha = 0;
            playButtonGroup.interactable = false;
        }
        
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);
        
        StartCoroutine(PlayIntroSequence());
        
        if (enablePreview && gameplayPreviewCamera != null)
        {
            StartCoroutine(RotatePreviewCamera());
        }
    }

    private IEnumerator PlayIntroSequence()
    {
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(AnimateLeaves());
        
        yield return new WaitForSeconds(titleFadeDelay);
        yield return StartCoroutine(FadeInTitle());
        
        yield return new WaitForSeconds(buttonFadeDelay);
        yield return StartCoroutine(FadeInPlayButton());
        
        animationComplete = true;
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
    }

    private IEnumerator FadeInTitle()
    {
        if (titleText == null) yield break;
        
        float elapsed = 0f;
        
        while (elapsed < titleFadeDuration)
        {
            elapsed += Time.deltaTime;
            titleText.alpha = Mathf.Lerp(0, 1, elapsed / titleFadeDuration);
            yield return null;
        }
        
        titleText.alpha = 1;
    }

    private IEnumerator FadeInPlayButton()
    {
        if (playButtonGroup == null) yield break;
        
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

    private IEnumerator RotatePreviewCamera()
    {
        if (gameplayPreviewCamera == null) yield break;
        
        Vector3 pivotPoint = previewCameraTarget != null ? previewCameraTarget.position : Vector3.zero;
        
        while (true)
        {
            gameplayPreviewCamera.transform.RotateAround(
                pivotPoint, 
                Vector3.up, 
                previewRotationSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    private void OnPlayButtonClicked()
    {
        if (!animationComplete) return;
        
        if (playButton != null)
            playButton.interactable = false;
        
        StartCoroutine(TransitionToGame());
    }

    private IEnumerator TransitionToGame()
    {
        if (playButtonGroup != null)
        {
            float elapsed = 0f;
            float fadeDuration = 0.3f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                playButtonGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
                yield return null;
            }
        }
        
        if (leftLeaf != null)
            Destroy(leftLeaf.gameObject);
        if (rightLeaf != null)
            Destroy(rightLeaf.gameObject);
        
        yield return new WaitForSeconds(sceneTransitionDelay);
        
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && animationComplete)
        {
            OnPlayButtonClicked();
        }
    }
}