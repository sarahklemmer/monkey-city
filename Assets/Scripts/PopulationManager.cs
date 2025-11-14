using UnityEngine;
using UnityEngine.Assertions;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager instance;
    
    [SerializeField] private GameObject monkeyPrefab;
    [SerializeField] private float zOffset = 0.2f;

    public int population {get; private set; } = 0;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate PopulationManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    public void AddToPopulation(int num)
    {
        Assert.IsTrue(num >= 0, "trying to spawn negative monkeys!");

        population += num;
        int xOffset = -num / 2;
        int count = num;
        while(count-- > 0)
        {
            xOffset += 1;
            float world_x = BuildingGrid.instance.GridXToWorldX(xOffset + Mathf.RoundToInt(BuildingGrid.instance.GetGridSize() / 2));
            float world_y = BuildingGrid.instance.WorldY();
            float world_z = BuildingGrid.instance.GridYToWorldZ(-2 + Mathf.RoundToInt(BuildingGrid.instance.GetGridSize() / 2)) + zOffset;
            GameObject monkey = Instantiate(monkeyPrefab, new Vector3(world_x, world_y + .7f, world_z), Quaternion.identity);
            // allocate monkey to tree of life instantly
            monkey.GetComponent<MonkeyController>().StartWalkingToBuilding(BuildingManager.instance.GetTreeOfLife());
        }
    }

    //TODO: actually make this more impactful on the state, it's pretty much useless
    public void RemoveFromPopulation(int num)
    {
        Assert.IsTrue(num >= 0, "trying to remove negative monkeys!");
        population -= num;
    }
}
