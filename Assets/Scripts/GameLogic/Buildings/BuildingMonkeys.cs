using System.Collections.Generic;
using UnityEngine.Assertions;

public class BuildingMonkeys
{
    public List<MonkeyController> monkeys;
    private HashSet<MonkeyController> enRoute = new();
    public readonly int capacity;

    public BuildingMonkeys(int capacity)
    {
        monkeys = new();
        this.capacity = capacity;
    }

    public int count => monkeys.Count;

    public bool CanAllocate()
    {
        enRoute.RemoveWhere(m => m == null || m.allocation == null || m.allocation.Allocated());
        return monkeys.Count + enRoute.Count < capacity;
    }

    public bool Add(MonkeyController m)
    {
        if (m == null || monkeys.Contains(m) || count >= capacity) return false;
        enRoute.Remove(m);
        monkeys.Add(m);
        return true;
    }

    public bool Remove(MonkeyController m)
    {
        if (m == null || !monkeys.Contains(m)) return false;
        monkeys.Remove(m);
        enRoute.Remove(m);
        return true;
    }

    public MonkeyController MonkeyToDeallocate()
    {
        return monkeys.Count > 0 ? monkeys[0] : null;
    }

    public void FreeMonkeys()
    {
        BuildingBase treeOfLife = BuildingManager.instance?.GetTreeOfLife();
        if (treeOfLife == null)
        {
            monkeys.Clear();
            return;
        }

        List<MonkeyController> temp = new List<MonkeyController>(monkeys);
        foreach (MonkeyController m in temp)
        {
            if (m != null && m.allocation != null)
            {
                m.allocation.Unassign();
                m.StartWalkingToBuilding(treeOfLife);
            }
        }
        Assert.IsTrue(monkeys.Count == 0, "didn't properly free all monkeys!");
    }

    public void StartWalking(MonkeyController m) { enRoute.Add(m); }

    public int Count() => monkeys.Count;
    public int Capacity() => capacity;
    public float CountOverCapacity() => ((float)Count()) / Capacity();
}