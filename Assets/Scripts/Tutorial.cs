using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject tutorialOption;
    public bool tutorialActive { get; private set; }
    public int tutorialStage { get; private set; }

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
        //TODO: stop chimps spawning
    }

    //check
    void BeginTutorial()
    {
        Assert.AreEqual(tutorialStage, 0, "calling BeginTutorial at the wrong stage!");
        tutorialOption.SetActive(false);
        ToastManager.Instance.RequestToast("Welcome to Monkey City! Click the building menu in the lower left and select Tree of Life to get started!", 4f);
        ++tutorialStage;
    }

    //check
    public void PlayerClickedTreeOfLifeButton()
    {
        Assert.AreEqual(tutorialStage, 1, "PlayerClickedTreeOfLifeButton called at wrong stage!");
        ToastManager.Instance.RequestToast("This is the center of your city. If it gets destroyed, you lose! It will also give you a new monkey every 3 days. Don't ask how. \nClick the indicator in the middle of the screen to place it!", 6f);
        ++tutorialStage;
    }

    //check
    public void PlayerPlacesTreeOfLife()
    {
        Assert.AreEqual(tutorialStage, 2, "PlayerPlacesTreeOfLife called at wrong stage!");
        ToastManager.Instance.RequestToast("Click the building menu again, but this time build a banana farm.", 3f);
        ++tutorialStage;
    }

    // check
    public void PlayerClickedBananaFarmButton()
    {
        Assert.AreEqual(tutorialStage, 3, "PlayerClickedBananaFarmButtonFirstTime called at wrong stage!");
        ToastManager.Instance.RequestToast("Don't see the placement indicator? Use the arrow keys to rotate the camera until you can find it.", 4f);
        ++tutorialStage;
    }

    // check
    public void PlayerPlacesBananaFarm()
    {
        Assert.AreEqual(tutorialStage, 4, "PlayerPlacesBananaFarm called at wrong stage!");
        ToastManager.Instance.RequestToast("You can click on any building to get its stats and a description of what it does. Try clicking the banana farm!", 4f);
        ++tutorialStage;
    }

    // check
    public void PlayerSelectsBananaFarm()
    {
        Assert.AreEqual(tutorialStage, 5, "PlayerSelectsBananaFarm called at wrong stage!");
        ToastManager.Instance.RequestToast("Right now, it won't produce anything because it doesn't have any monkeys assigned to it. Click anywhere outside of the window to close it.", 5f);
        ++tutorialStage;
    }

    // check
    public void PlayerClosesBananaFarmWindow()
    {
        Assert.AreEqual(tutorialStage, 6, "PlayerClosesBananaFarmWindow called at wrong stage!");
        ToastManager.Instance.RequestToast("Left click a monkey and right click the banana farm to assign that monkey to the building. It should go inside the building once it arrives.", 5f);
        ++tutorialStage;
    }


    // check
    public void FirstMonkeyAllocatedToFarm()
    {
        Assert.AreEqual(tutorialStage, 7, "FirstMonkeyAllocatedToFarm called at wrong stage!");
        ToastManager.Instance.RequestToast("Click on the building again to see its updated stats, and then allocate another monkey", 3f);
        ++tutorialStage;
    }

    // check
    public void PlayerClosesBananaFarmWindowAgainAndAllocatesMonkey()
    {
        Assert.AreEqual(tutorialStage, 8, "PlayerClosesBananaFarmWindowAgainAndAllocatesMonkey called at wrong stage!");
        TimeController.instance.StartTicking();
        ToastManager.Instance.RequestToast("It would be nice if your monkeys could just sit around farming bananas all day, but this jungle is also home to evil chimps who are attracted to banana-rich settlements. Once a day passes and you have enough bananas, place an archer tower using the building menu and assign a monkey to it to defend yourself", 7f);
        ++tutorialStage;
    }

    // check
    public void PlayerPlacedAndAllocatedArcherTower()
    {
        Assert.AreEqual(tutorialStage, 9, "PlayerPlacedAndAllocatedArcherTower called at wrong stage!");
        // TODO: spawn chimp
        ++tutorialStage;
    }

    public void ChimpKilledByTower()
    {
        Assert.AreEqual(tutorialStage, 10, "ChimpKilledByTower called at wrong stage!");
        ToastManager.Instance.RequestToast("As your base gets bigger, bigger waves of chimps will come and try to destroy your village! If they get your tree of life, your city is destroyed and you'll have to rebuild from scratch. Make sure your defenses scale with your production, and make sure you upgrade your archer towers by clicking on them when you get the chance.", 5f);
        ++tutorialStage;
    }

    public void BeginTutorialButtonClick()
    {
        tutorialOption.SetActive(false);
        BeginTutorial();
    }

    public void SkipTutorialButtonClick()
    {
        tutorialActive = false;
        tutorialOption.SetActive(false);
        TimeController.instance.StartTicking();
        //TODO: start chimps spawning
    }
}