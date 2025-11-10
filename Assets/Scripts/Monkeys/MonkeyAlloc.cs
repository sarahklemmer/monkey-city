using UnityEngine;
using UnityEngine.Assertions;

public class MonkeyAlloc : MonoBehaviour
{
    BuildingBase current = null;
    MonkeyController self = null;

    public bool Allocated() => current != null;
    public BuildingBase CurrentBuilding() => current;

    public void Initialize(MonkeyController self)
    {
        this.self = self;
    }

    public void Allocate(BuildingBase next)
    {
        if (Tutorial.instance.tutorialActive)
        {
            switch(Tutorial.instance.tutorialStage)
            {
                case 7:
                    if (next.GetBuildingType() == BuildingType.BananaFarm) Tutorial.instance.FirstMonkeyAllocatedToFarm();
                    break;
                case 8:
                    if (next.GetBuildingType() == BuildingType.BananaFarm) Tutorial.instance.PlayerClosesBananaFarmWindowAgainAndAllocatesMonkey();
                    break;
                case 9:
                    if (next.GetBuildingType() == BuildingType.ArcherTower) Tutorial.instance.PlayerPlacedAndAllocatedArcherTower();
                    break;
            }
        }
        Assert.IsNotNull(self, "trying to use uninitialized MonkeyAlloc!");
        if (next == null) return;
        // try to add self to building
        if (!next.AddMonkey(self)) return;
        current = next;
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        if (MonkeySelector.instance != null)
        {
            MonkeySelector.instance.DeselectIfSelected(self);
        }
    }
    
    public void Deallocate()
    {
        if (current == null) return;
        current.RemoveMonkey(self);
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        current = null;
    }
}
