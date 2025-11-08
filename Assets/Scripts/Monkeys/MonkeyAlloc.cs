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
        Assert.IsNotNull(self, "trying to use uninitialized MonkeyAlloc!");
        if (next == null) return;
        // try to add self to building
        if (!next.AddMonkey(self)) return;
        current = next;
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
    }
    
    public void Deallocate()
    {
        if (current == null) return;
        current.RemoveMonkey(self);
        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;
        current = null;
    }
}
