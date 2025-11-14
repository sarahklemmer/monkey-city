using UnityEngine;

public class SimpleTutorial : MonoBehaviour
{
    bool tutorialActive = false;
    int stage = 0;
    public static SimpleTutorial instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate SimpleTutorial on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void StartTutorial() {
        tutorialActive = true;
        TimeController.instance.StopTicking();
    }

    // Update is called once per frame
    void Update()
    {
        // lowkey i wouldn't even try to read this sorry guys
        if (!tutorialActive) return;
        switch(stage)
        {
            case 0:
                if (BuildingSelector.instance.BuildingSelected() && BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm).Count > 0)
                {
                    TextOverBuilding.instance.DisplayText(BuildingManager.instance.GetTreeOfLife().gameObject, "first, left click me!");
                    TextOverBuilding.instance.DisplayText(BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm)[0].gameObject, "then, right click me!");
                    ++stage;
                }
                break;
            case 1:
                if (BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm)[0].GetMonkeyCount() > 0)
                {
                    TextOverBuilding.instance.DisableAllText();
                    TextOverBuilding.instance.DisplayText(BuildingManager.instance.GetBuildingsOfType(BuildingType.BananaFarm)[0].gameObject, "shift click me!");
                    ++stage;
                }
                break;
            case 2:
                if (BuildingInfo.instance.ShownAndHidden())
                {
                    TextOverBuilding.instance.DisableAllText();
                    ToastManager.Instance.RequestToast("you can move monkeys between any two buildings this way. Good luck!");
                    tutorialActive = false;
                    TimeController.instance.StartTicking();
                }
                break;
        }
    }
}
