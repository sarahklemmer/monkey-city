using UnityEngine;
using System.Collections;

public class DayNightToggle : MonoBehaviour
{
    [SerializeField] Light sun;
    [SerializeField] Light moon;
    [SerializeField] Material daySkybox;
    [SerializeField] Material nightSkybox;
    [SerializeField] float daySunIntensity = 1.2f;
    [SerializeField] float nightSunIntensity = 0.05f;
    [SerializeField] float moonIntensity = 0.25f;
    [SerializeField] float transitionDuration = 2f;
    [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Color daySunColor = new Color(1.0f, 0.96f, 0.84f);
    private Color nightSunColor = new Color(0.55f, 0.65f, 0.9f);

    bool isNight = false;
    private Coroutine transitionCoroutine;

    public static DayNightToggle instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate DayNightToggle on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        if (sun != null)
        {
            RenderSettings.sun = sun;
            SetNightImmediate(false);
        }
        else
        {
            Debug.LogWarning("DayNightToggle: Sun light is not assigned in the Inspector!");
        }
    }

    public void Toggle()
    {
        SetNight(!isNight);
    }

    public void SetNight(bool night)
    {
        if (isNight == night) return;
        
        isNight = night;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionToState(night));
    }

    public void SetNightImmediate(bool night)
    {
        isNight = night;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        ApplyStateImmediate(night);
    }

    private void ApplyStateImmediate(bool night)
    {
        if (sun == null) return;
        
        sun.color = night ? nightSunColor : daySunColor;
        sun.intensity = night ? nightSunIntensity : daySunIntensity;
        sun.shadows = LightShadows.Soft;

        if (moon != null)
        {
            moon.gameObject.SetActive(night);
            moon.intensity = night ? moonIntensity : 0f;
        }

        RenderSettings.sun = night ? moon : sun;

        if (daySkybox && nightSkybox)
            RenderSettings.skybox = night ? nightSkybox : daySkybox;

        DynamicGI.UpdateEnvironment();
    }

    private IEnumerator TransitionToState(bool toNight)
    {
        if (sun == null) yield break;
        
        float elapsed = 0f;

        Color startSunColor = sun.color;
        float startSunIntensity = sun.intensity;
        float startMoonIntensity = moon != null ? moon.intensity : 0f;

        Color targetSunColor = toNight ? nightSunColor : daySunColor;
        float targetSunIntensity = toNight ? nightSunIntensity : daySunIntensity;
        float targetMoonIntensity = toNight ? moonIntensity : 0f;

        if (moon != null && toNight)
        {
            moon.gameObject.SetActive(true);
        }

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float curveT = transitionCurve.Evaluate(t);

            sun.color = Color.Lerp(startSunColor, targetSunColor, curveT);
            sun.intensity = Mathf.Lerp(startSunIntensity, targetSunIntensity, curveT);

            if (moon != null)
            {
                moon.intensity = Mathf.Lerp(startMoonIntensity, targetMoonIntensity, curveT);
            }

            if (t >= 0.5f && daySkybox && nightSkybox)
            {
                RenderSettings.skybox = toNight ? nightSkybox : daySkybox;
            }

            yield return null;
        }

        sun.color = targetSunColor;
        sun.intensity = targetSunIntensity;
        sun.shadows = LightShadows.Soft;

        if (moon != null)
        {
            moon.intensity = targetMoonIntensity;
            moon.gameObject.SetActive(toNight);
        }

        RenderSettings.sun = toNight ? moon : sun;

        if (daySkybox && nightSkybox)
            RenderSettings.skybox = toNight ? nightSkybox : daySkybox;

        DynamicGI.UpdateEnvironment();

        transitionCoroutine = null;
    }

    public bool IsNight() => isNight;
    public bool IsTransitioning() => transitionCoroutine != null;
}