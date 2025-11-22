using UnityEngine;
using UnityEngine.Assertions;

public class DeathScreen : MonoBehaviour
{
    [SerializeField] GameObject deathScreen;
    [SerializeField] Transform exceptionRoot;
    public static DeathScreen instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate DeathScreen on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        
        Assert.IsNotNull(deathScreen, "forgot to add death screen in inspector!");
        deathScreen.SetActive(false);
        instance = this;
    }

    public void ShowDeathScreen()
    {
        UIInteractabilityManager.instance.DisableInteractivityExcept(exceptionRoot);
        deathScreen.SetActive(true);
    }

    public void OnClick()
    {
        SceneLoader.instance.ReloadScene();
    }
}
