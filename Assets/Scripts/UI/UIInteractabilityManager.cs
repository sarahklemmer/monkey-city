using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIInteractabilityManager : MonoBehaviour
{
    public static UIInteractabilityManager instance;

    private CanvasGroup cg;
    private float defaultAlpha;
    private CanvasGroup lastException = null;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate UIInteractabilityManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
    
        instance = this;
        cg = GetComponent<CanvasGroup>();
        defaultAlpha = cg.alpha;
    }

    public void DisableUI()
    {
        DisableLastExceptionIgnoringParentGroups();
        BuildingSelector.instance.DisableSelection();

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        PauseGame();
    }

    public void EnableUI()
    {
        DisableLastExceptionIgnoringParentGroups();
        BuildingSelector.instance.EnableSelection();

        cg.alpha = defaultAlpha;
        cg.interactable = true;
        cg.blocksRaycasts = true;

        UnpauseGame();
    }

    public void DisableInteractivity()
    {
        DisableLastExceptionIgnoringParentGroups();
        DisableInteractivityInternal();
    }

    public void EnableInteractivity()
    {
        DisableLastExceptionIgnoringParentGroups();
        cg.interactable = true;
        UnpauseGame();
    }

    public void DisableInteractivityExcept(Transform exceptionRoot, bool pauseGame = true)
    {   
        DisableLastExceptionIgnoringParentGroups();
        if(exceptionRoot != null)
        {
            lastException = exceptionRoot.GetComponent<CanvasGroup>();
            if(lastException == null) lastException = exceptionRoot.gameObject.AddComponent<CanvasGroup>();
            lastException.ignoreParentGroups = true;
            lastException.interactable = true;
        }
        DisableInteractivityInternal(pauseGame);
    }

    private void DisableLastExceptionIgnoringParentGroups()
    {
        if(lastException != null) lastException.ignoreParentGroups = false; 
        lastException = null;
    }

    private void DisableInteractivityInternal(bool pauseGame = true)
    {
        if(pauseGame) PauseGame();
        cg.interactable = false;
    }

    private void PauseGame()
    {
        TimeController.instance.StopTicking();
        BananaProductionTimer.instance.StopProduction();
        WaveSpawner.instance.PauseSpawning();
    }

    private void UnpauseGame()
    {
        TimeController.instance.StartTicking();
        BananaProductionTimer.instance.StartProduction();
        WaveSpawner.instance.UnpauseSpawning();
    }
}