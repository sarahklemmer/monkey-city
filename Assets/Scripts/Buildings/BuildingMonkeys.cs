using System.Collections.Generic;

using System.Collections.Generic;

public class BuildingMonkeys
{
    List<MonkeyController> monkeys;
    public readonly int capacity;

    public BuildingMonkeys(int capacity)
    {
        monkeys = new();
        this.capacity = capacity;
    }

    // ADD THIS PROPERTY
    public int count => monkeys.Count;

    public bool CanAllocate()
    {
        return monkeys.Count < capacity;
    }

    public bool Add(MonkeyController m)
    {
        if (m == null || !CanAllocate() || monkeys.Contains(m)) return false;
        monkeys.Add(m);
        return true;
    }

    public bool Remove(MonkeyController m)
    {
        if (m == null || !monkeys.Contains(m)) return false;
        monkeys.Remove(m);
        return true;
    }

    public MonkeyController MonkeyToDeallocate()
    {
        return monkeys.Count > 0 ? monkeys[0] : null;
    }
}