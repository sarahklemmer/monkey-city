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

    public bool Assign(BuildingBase next)
    {
        Assert.IsNotNull(self, "trying to use uninitialized MonkeyAlloc!");
        if (next == null) return false;
        // try to add self to building
        if (!next.AddMonkey(self)) return false;
        current = next;
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        return true;

    }
    
    public void Unassign()
    {
        if (current == null) return;
        current.RemoveMonkey(self);
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        current = null;
    }
}
