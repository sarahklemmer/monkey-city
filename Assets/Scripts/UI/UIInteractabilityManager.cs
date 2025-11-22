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
        BuildingMenuManager.instance.HideAllMenus();

        BuildingSelector.instance.DisableSelection();
        BuildingMenuManager.instance.HideAllMenus();

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void EnableUI()
    {
        DisableLastExceptionIgnoringParentGroups();
        BuildingSelector.instance.EnableSelection();

        cg.alpha = defaultAlpha;
        cg.interactable = true;
        cg.blocksRaycasts = true;
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
    }

    public void DisableInteractivityExcept(Transform exceptionRoot)
    {   
        DisableLastExceptionIgnoringParentGroups();
        if(exceptionRoot != null)
        {
            lastException = exceptionRoot.GetComponent<CanvasGroup>();
            if(lastException == null) lastException = exceptionRoot.gameObject.AddComponent<CanvasGroup>();
            lastException.ignoreParentGroups = true;
            lastException.interactable = true;
        }
        DisableInteractivityInternal();
    }

    private void DisableLastExceptionIgnoringParentGroups()
    {
        if(lastException != null) lastException.ignoreParentGroups = false; 
        lastException = null;
    }

    private void DisableInteractivityInternal()
    {
        cg.interactable = false;
    }
}