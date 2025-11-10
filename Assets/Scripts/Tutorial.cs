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

    //check
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

    //check
    public void PlayerClickedTreeOfLifeButton()
    {
        Assert.AreEqual(tutorialStage, 1, "PlayerClickedTreeOfLifeButton called at wrong stage!");
        message = "This is the center of your city. If it gets destroyed, you lose! It will also give you a new monkey every 2 days. Don't ask how.\nClick the indicator in the middle of the screen to place it!";
        duration = 6f;
        Toast();
        ++tutorialStage;
    }

    //check
    public void PlayerPlacesTreeOfLife()
    {
        Assert.AreEqual(tutorialStage, 2, "PlayerPlacesTreeOfLife called at wrong stage!");
        message = "Click the building menu again, but this time build a banana farm.";
        duration = 3f;
        Toast();
        ++tutorialStage;
    }

    // check
    public void PlayerClickedBananaFarmButton()
    {
        Assert.AreEqual(tutorialStage, 3, "PlayerClickedBananaFarmButtonFirstTime called at wrong stage!");
        message = "Don't see the placement indicator? Use the arrow keys to rotate the camera until you can find it.";
        duration = 3f;
        Toast();
        ++tutorialStage;
    }

    // check
    public void PlayerPlacesBananaFarm()
    {
        Assert.AreEqual(tutorialStage, 4, "PlayerPlacesBananaFarm called at wrong stage!");
        message = "You can click on any building to get its stats and a description of what it does. Try clicking the banana farm!";
        duration = 3f;
        Toast();
        ++tutorialStage;
    }

    // check
    public void PlayerSelectsBananaFarm()
    {
        Assert.AreEqual(tutorialStage, 5, "PlayerSelectsBananaFarm called at wrong stage!");
        message = "Right now, it won't produce anything because it doesn't have any monkeys assigned to it. Click anywhere outside of the window to close it.";
        duration = 5f;
        Toast();
        ++tutorialStage;
    }

    // check
    public void PlayerClosesBananaFarmWindow()
    {
        Assert.AreEqual(tutorialStage, 6, "PlayerClosesBananaFarmWindow called at wrong stage!");
        message = "Left click a monkey and right click the banana farm to assign that monkey to the building. It should go inside the building once it arrives.";
        duration = 5f;
        Toast();
        ++tutorialStage;
    }

    // check
    public void FirstMonkeyAllocatedToFarm()
    {
        Assert.AreEqual(tutorialStage, 7, "FirstMonkeyAllocatedToFarm called at wrong stage!");
        message = "Click on the building again to see its updated stats, and then allocate another monkey.";
        duration = 2f;
        Toast();
        ++tutorialStage;
    }

    // check
    public void PlayerClosesBananaFarmWindowAgainAndAllocatesMonkey()
    {
        Assert.AreEqual(tutorialStage, 8, "PlayerClosesBananaFarmWindowAgainAndAllocatesMonkey called at wrong stage!");
        message = "We've given you some bananas to place an archer tower using the building menu. Assign a monkey to it to defend yourself from the other residents of the jungle...";
        duration = 6f;
        Toast();
        BananaManager.instance.AddBananas(4);
        ++tutorialStage;
    }

    // check
    public void PlayerPlacedAndAllocatedArcherTower()
    {
        Assert.AreEqual(tutorialStage, 9, "PlayerPlacedAndAllocatedArcherTower called at wrong stage!");
        message = "Your archer towers will only defend when manned.";
        duration = 1f;
        Toast();
        Instantiate(enemyPrefab, new Vector3(BuildingGrid.instance.GridXToWorldX(0) - 2f, 1f, BuildingGrid.instance.GridYToWorldZ(0) - 2f), Quaternion.identity);
        ++tutorialStage;
    }

    // check
    public void ChimpKilledByTower()
    {
        Assert.AreEqual(tutorialStage, 10, "ChimpKilledByTower called at wrong stage!");
        duration = 4.5f;
        message = "As your base gets bigger, bigger waves of chimps will come and try to destroy your village! If they get your tree of life, your city is destroyed and you'll have to rebuild from scratch.";
        Toast();
        message = "Make sure your defenses scale with your production, and make sure you upgrade your archer towers by clicking on them when you get the chance. Good luck!";
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
}