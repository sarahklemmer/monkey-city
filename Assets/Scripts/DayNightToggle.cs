using UnityEngine;

public class DayNightToggle : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] Light sun;       // your Directional Light
    [SerializeField] Light moon;      // optional: a cooler Directional Light

    [Header("Skyboxes (optional)")]
    [SerializeField] Material daySkybox;
    [SerializeField] Material nightSkybox;

    [Header("Intensities")]
    [SerializeField] float daySunIntensity = 1.2f;
    [SerializeField] float nightSunIntensity = 0.05f;
    [SerializeField] float moonIntensity = 0.25f;

    bool isNight = false;

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
        RenderSettings.sun = sun;
        SetNight(false);
    }

    public void Toggle()
    {
        SetNight(!isNight);
    }

    public void SetNight(bool night)
    {
        isNight = night;


        sun.color = night ? new Color(0.55f, 0.65f, 0.9f) : new Color(1.0f, 0.96f, 0.84f);
        sun.intensity = night ? nightSunIntensity : daySunIntensity;
        sun.shadows = LightShadows.Soft;

        moon.gameObject.SetActive(night);
        moon.intensity = night ? moonIntensity : 0f;
    
        // Assign the active "sun" for URP shading
        RenderSettings.sun = night ? moon : sun;

        // Skybox (optional)
        if (daySkybox && nightSkybox)
            RenderSettings.skybox = night ? nightSkybox : daySkybox;

        // Update ambient from skybox
        DynamicGI.UpdateEnvironment();
    }
}