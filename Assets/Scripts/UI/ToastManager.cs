using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public struct ToastData
{
    public string message;
    public float duration;
    public float fadeDuration;
    public bool waitForInput;
    public ToastData(string message, float duration = 2.0f, float fadeDuration = 0.5f, bool waitForInput = false)
    {
        this.message = message;
        this.duration = duration;
        this.fadeDuration = fadeDuration;
        this.waitForInput = waitForInput;
    }
}

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance;
    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private GameObject toastPanel;
    [SerializeField] private CanvasGroup toastCanvasGroup;
    [SerializeField] private AudioSource toastAudioSource;
    [SerializeField] private AudioClip toastSound;
    [SerializeField] private GameObject continuePrompt;

    Queue<ToastData> toastQueue = new Queue<ToastData>();

    private bool fading = false;
    private bool isShowingToast = false;
    private bool waitingForInput = false;

    public bool freezeToasts = false;

    void Start()
    {
        toastPanel.SetActive(false);
        continuePrompt.SetActive(false);
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        continuePrompt.SetActive(isShowingToast && !fading);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ForceEndCurrentToast();
        }
        if (!freezeToasts)
        {
            ProcessToastQueue();
        }
    }

    public void RequestToast(string message, float duration = 2.0f, float fadeDuration = 0.5f, bool waitForInput = false)
    {
        ToastData toastData = new ToastData(message, duration, fadeDuration, waitForInput);
        toastQueue.Enqueue(toastData);
    }

    public void SkipToNextToast()
    {
        if (waitingForInput)
        {
            waitingForInput = false;
            StopAllCoroutines();
            StartCoroutine(FadeOutAndNext());
        }
    }
    public void ForceEndCurrentToast()
    {
        if (isShowingToast)
        {
            StopAllCoroutines();
            toastCanvasGroup.alpha = 0f;
            toastPanel.SetActive(false);
            isShowingToast = false;
            waitingForInput = false;
        }
    }

    private void ProcessToastQueue()
    {
        if (!isShowingToast && toastQueue.Count > 0)
        {
            ToastData nextToast = toastQueue.Dequeue();
            StartCoroutine(DoToast(nextToast));
        }
    }

    IEnumerator DoToast(ToastData data)
    {
        if (isShowingToast)
        {
            yield break;
        }
        isShowingToast = true;
        toastText.text = data.message;
        toastPanel.SetActive(true);

        float initialTime = Time.time;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            progress = (Time.time - initialTime) / data.fadeDuration;
            toastCanvasGroup.alpha = Mathf.Lerp(0, 1, progress);
            yield return null;
        }
        toastCanvasGroup.alpha = 1.0f;

        if (data.waitForInput)
        {
            waitingForInput = true;
            
            // Wait until spacebar is pressed (handled in Update)
            while (waitingForInput)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(data.duration);
        }

        initialTime = Time.time;
        progress = 0.0f;

        while (progress < 1.0f)
        {
            progress = (Time.time - initialTime) / data.fadeDuration;
            toastCanvasGroup.alpha = Mathf.Lerp(1, 0, progress);
            yield return null;
        }

        toastCanvasGroup.alpha = 0.0f;
        toastPanel.SetActive(false);
        isShowingToast = false;
    }
    IEnumerator FadeOutAndNext()
    {
        fading = true;
            
        float initialTime = Time.time;
        float progress = 0.0f;
        float quickFade = 0.2f;

        while (progress < 1.0f)
        {
            progress = (Time.time - initialTime) / quickFade;
            toastCanvasGroup.alpha = Mathf.Lerp(toastCanvasGroup.alpha, 0, progress);
            yield return null;
        }

        toastCanvasGroup.alpha = 0.0f;
        toastPanel.SetActive(false);
        isShowingToast = false;
        fading = false;
    }
}
