using UnityEngine;
using UnityEngine.Assertions;

public class PlaceBuildingButton : MonoBehaviour
{
    BuildingType type;
    private bool initialized = false;

    [SerializeField] private TutorialManager tutorialManager;

    public void Initialize(BuildingType type)
    {
        this.type = type;
        initialized = true;
    }

    public void OnClick()
    {
        Debug.Log($"=== BUTTON CLICKED === Type: {type}");
        Assert.IsTrue(initialized, "trying to click non initialized button");

        if (BananaManager.instance.GetBananas() < BuildingTypeToPrice.GetPrice(type)) return;
        // can only place 5 of all types
        if (BuildingManager.instance.GetBuildingsOfType(type).Count >= 5) return;
        
        if (type == BuildingType.Wall)
        {
            BananaManager.instance.RemoveBananas(BuildingTypeToPrice.GetPrice(type));

            BuildingGrid.instance.RevealPresetWalls();
            BuildingGrid.instance.FinishLevel();

            PlacementManager.instance.ClearCurrentBuilding();
            return;
        }
        if (tutorialManager == null)
        {
            tutorialManager = FindFirstObjectByType<TutorialManager>();
        }
        Debug.Log($"isActive: {tutorialManager.isActive}, currentStepIndex: {tutorialManager.currentStepIndex}");
        if (tutorialManager.isActive && tutorialManager.currentStepIndex == 0)
        {
            Debug.Log($"Tutorial active, completing step {tutorialManager.currentStepIndex}");
            tutorialManager.OnStepCompleted();
        }
        if (tutorialManager.isActive && tutorialManager.currentStepIndex == 2)
        {
            Debug.Log($"Tutorial active, completing step {tutorialManager.currentStepIndex}");
            tutorialManager.OnStepCompleted();
        }

        PlacementManager.instance.SetCurrentBuilding(type);
    }
}