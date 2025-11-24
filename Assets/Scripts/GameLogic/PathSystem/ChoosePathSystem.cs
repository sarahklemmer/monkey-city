using UnityEngine;

public class ChoosePathSystem : MonoBehaviour
{
    public static ChoosePathSystem instance;

    [SerializeField] private GameObject choosePathMenu;
    [SerializeField] private GameObject playerPointDisplay;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate ChoosePathSystem on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        if (playerPointDisplay != null)
        {
            playerPointDisplay.SetActive(false);
        }
    }

    public void ShowPathMenu()
    {
        TimeController.instance.StopTicking();
        BananaProductionTimer.instance.StopProduction();
        WaveSpawner.instance.PauseSpawning();

        choosePathMenu.SetActive(true);
    }

    public void HidePathMenu()
    {
        TimeController.instance.StartTicking();
        BananaProductionTimer.instance.StartProduction();
        WaveSpawner.instance.UnpauseSpawning();

        choosePathMenu.SetActive(false);

        if (playerPointDisplay != null)
        {
            playerPointDisplay.SetActive(true);
        }

        ToastManager.Instance?.ForceEndCurrentToast();

        ToastManager.Instance?.RequestToast("Remember to upgrade path levels in the tree of life!", 2f);
    }

    public void ChooseFarmerPath()
    {
        PathManager.instance.SelectFarmerPath();
        HidePathMenu();
    }

    public void ChooseWarriorPath()
    {
        PathManager.instance.SelectWarriorPath();
        HidePathMenu();
    }

    public void ChooseScholarPath()
    {
        PathManager.instance.SelectScholarPath();
        HidePathMenu();
    }
}