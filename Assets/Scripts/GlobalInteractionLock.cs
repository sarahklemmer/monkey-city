using UnityEngine;
using UnityEngine.Assertions;

public class GlobalInteractionLock : MonoBehaviour
{
    private bool locked = false;
    public static GlobalInteractionLock instance;
    [SerializeField] GameObject buildingToggle;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate GlobalInteractionLock on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        Assert.IsNotNull(buildingToggle, "forgot to set building toggle in Global Interaction Lock");
    }

    public static void Lock() {
        instance.buildingToggle.SetActive(false);
        BuildingSelector.instance.DisableSelection();
        instance.locked = true;
    }

    public static void Unlock() { 
        instance.buildingToggle.SetActive(true);
        BuildingSelector.instance.EnableSelection();
        instance.locked = false; 
    }

    public static bool IsLocked() => instance.locked;
}
