using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject tutorialOption;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject repeatButton;

    public bool tutorialActive { get; private set; }
    public int tutorialStage { get; private set; }
    string message;
    float duration;

    public static Tutorial instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate Tutorial on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;

        tutorialActive = true;
        tutorialStage = 0;
    }

    void Start()
    {
        tutorialOption.GetComponentInChildren<TMP_Text>().text = "Welcome to Monkey City!";
        TimeController.instance.StopTicking();
        WaveSpawner.instance.PauseSpawning();
        repeatButton.SetActive(false);
    }

    void BeginTutorial()
    {
        Assert.AreEqual(tutorialStage, 0, "calling BeginTutorial at the wrong stage!");
        tutorialOption.SetActive(false);
        repeatButton.SetActive(true);
        message = "Welcome to Monkey City! Click the building menu in the lower left and select Tree of Life to get started!"; 
        duration = 4f;
        Toast();
        ++tutorialStage;
    }
    
    public void PlayerClickedTreeOfLifeButton()
    {
        if (tutorialStage != 1)
        {
            Debug.LogWarning($"PlayerClickedTreeOfLifeButton called at wrong stage! Expected 1, got {tutorialStage}");
            return;
        }
        
        message = "This is the center of your city. If it gets destroyed, you lose! It will also give you a new monkey every 2 days. Don't ask how.\nClick the indicator in the middle of the screen to place it!";
        duration = 6f;
        Toast();
        ++tutorialStage;
    }

    public void PlayerPlacesTreeOfLife()
    {
        if (tutorialStage != 2)
        {
            Debug.LogWarning($"PlayerPlacesTreeOfLife called at wrong stage! Expected 2, got {tutorialStage}");
            return;
        }
        
        message = "Click the building menu again, but this time build a banana farm.";
        duration = 3f;
        Toast();
        ++tutorialStage;
    }

    public void PlayerClickedBananaFarmButton()
    {
        if (tutorialStage != 3)
        {
            Debug.LogWarning($"PlayerClickedBananaFarmButton called at wrong stage! Expected 3, got {tutorialStage}");
            return;
        }
        
        message = "Don't see the placement indicator? Use the arrow keys to rotate the camera until you can find it.";
        duration = 3f;
        Toast();
        ++tutorialStage;
    }

    public void PlayerPlacesBananaFarm()
    {
        if (tutorialStage != 4)
        {
            Debug.LogWarning($"PlayerPlacesBananaFarm called at wrong stage! Expected 4, got {tutorialStage}");
            return;
        }
        
        message = "You can click on any building to get its stats and a description of what it does. Try clicking the banana farm!";
        duration = 3f;
        Toast();
        ++tutorialStage;
    }

    public void PlayerSelectsBananaFarm()
    {
        if (tutorialStage != 5)
        {
            Debug.LogWarning($"PlayerSelectsBananaFarm called at wrong stage! Expected 5, got {tutorialStage}");
            return;
        }
        
        message = "Right now, it won't produce anything because it doesn't have any monkeys assigned to it. Click anywhere outside of the window to close it.";
        duration = 5f;
        Toast();
        ++tutorialStage;
    }

    public void PlayerClosesBananaFarmWindow()
    {
        if (tutorialStage != 6)
        {
            Debug.LogWarning($"PlayerClosesBananaFarmWindow called at wrong stage! Expected 6, got {tutorialStage}");
            return;
        }
        
        message = "Left click a monkey and right click the banana farm to assign that monkey to the building. It should go inside the building once it arrives. Hint: if you're having trouble selecting things, try rotating the camera so that you have an unobstructed line of sight";
        duration = 5f;
        Toast();
        ++tutorialStage;
    }

    public void FirstMonkeyAllocatedToFarm()
    {
        if (tutorialStage != 7)
        {
            Debug.LogWarning($"FirstMonkeyAllocatedToFarm called at wrong stage! Expected 7, got {tutorialStage}");
            return;
        }
        
        message = "Click on the building again to see its updated stats, and then allocate another monkey.";
        duration = 2f;
        Toast();
        ++tutorialStage;
    }

    public void PlayerClosesBananaFarmWindowAgainAndAllocatesMonkey()
    {
        if (tutorialStage != 8)
        {
            Debug.LogWarning($"PlayerClosesBananaFarmWindowAgainAndAllocatesMonkey called at wrong stage! Expected 8, got {tutorialStage}");
            return;
        }
        
        message = "We've given you some bananas to place an archer tower using the building menu. Assign a monkey to it to defend yourself from the other residents of the jungle...";
        duration = 6f;
        Toast();
        BananaManager.instance.AddBananas(4);
        ++tutorialStage;
    }

    public void PlayerPlacedAndAllocatedArcherTower()
    {
        if (tutorialStage != 9)
        {
            Debug.LogWarning($"PlayerPlacedAndAllocatedArcherTower called at wrong stage! Expected 9, got {tutorialStage}");
            return;
        }
        
        message = "Your archer towers will only defend when manned.";
        duration = 1f;
        Toast();
        Instantiate(enemyPrefab, new Vector3(BuildingGrid.instance.GridXToWorldX(0) - 2f, 1f, BuildingGrid.instance.GridYToWorldZ(0) - 2f), Quaternion.identity);
        ++tutorialStage;
    }

    public void ChimpKilledByTower()
    {
        if (tutorialStage != 10)
        {
            Debug.LogWarning($"ChimpKilledByTower called at wrong stage! Expected 10, got {tutorialStage}");
            return;
        }
        
        repeatButton.SetActive(false);
        
        duration = 4.5f;
        message = "As your base gets bigger, bigger waves of chimps will come and try to destroy your village! If they get your tree of life, your city is destroyed and you'll have to rebuild from scratch.";
        Toast();
        duration = 6f;
        message = "Make sure your defenses scale with your production, and make sure you upgrade your archer towers by clicking on them when you get the chance. Once your farms have made 75 bananas, chimps will start coming in increasing waves. Good luck!";
        Toast();
        FinishTutorial();
    }

    public void BeginTutorialButtonClick()
    {
        tutorialOption.SetActive(false);
        BeginTutorial();
    }

    public void SkipTutorialButtonClick()
    {
        FinishTutorial();
    }

    void FinishTutorial()
    {
        tutorialActive = false;
        tutorialOption.SetActive(false);
        repeatButton.SetActive(false);
        TimeController.instance.StartTicking();
        WaveSpawner.instance.UnpauseSpawning();
    }

    public void Toast()
    {
        ToastManager.Instance.RequestToast(message, duration);
        ToastManager.Instance.SkipToNextToast();
    }

    public void RepeatCurrentInstruction()
    {
        if (tutorialActive && !string.IsNullOrEmpty(message))
        {
            ToastManager.Instance.RequestToast(message, duration);
            ToastManager.Instance.SkipToNextToast();
        }
    }

    public bool IsOnStep(int step)
    {
        return tutorialActive && tutorialStage == step;
    }

    public void ShowStepMismatchWarning(string actionName, int expectedStep)
    {
        if (tutorialActive && tutorialStage != expectedStep)
        {
            string warningMsg = $"Please complete the current tutorial step first!";
            ToastManager.Instance.RequestToast(warningMsg, 2f);
        }
    }
}