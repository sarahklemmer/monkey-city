using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject tutorialOption;

    void Start()
    {
        tutorialOption.GetComponentInChildren<TMP_Text>().text = "Welcome to Monkey City!";
        TimeController.instance.StopTicking();
        
    }

    public void BeginTutorial()
    {
        ToastManager.Instance.RequestToast("Welcome to Monkey City! Click the building menu in the lower left and select Tree of Life to get started!");
    }

    public void PlayerClickedTreeOfLifeButton()
    {
        ToastManager.Instance.RequestToast("This is the center of your city. If it gets destroyed, you lose! It will also give you a new monkey every 3 days. Don't ask how. \nClick the indicator in the middle of the screen to place it!");
    }

    public void PlayerPlacesTreeOfLife()
    {
        ToastManager.Instance.RequestToast("Click the building menu again, but this time build a banana farm.");
    }
}
