using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager instance;
    
    [SerializeField] private GameObject monkeyPrefab;
    [SerializeField] private TextMeshProUGUI monkeyCountText;
    [SerializeField] private int monkeyCount = 0;
    [SerializeField] private float zOffset = 0.2f;
    
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

    void Start()
    {
        monkeyCount = 0;
        AddToPopulation(0);
    }
    
    public void AddToPopulation(int num)
    {
        Assert.IsNotNull(monkeyCountText, "Assign a monkeyCountText in the Inspector!");
        Assert.IsTrue(num >= 0, "trying to spawn negative monkeys!");

        int xOffset = -num / 2;
        int count = num;
        while(count-- > 0)
        {
            xOffset += 1;
            float world_x = BuildingGrid.instance.GridXToWorldX(xOffset + Mathf.RoundToInt(BuildingGrid.instance.GetGridSize() / 2));
            float world_y = BuildingGrid.instance.WorldY();
            float world_z = BuildingGrid.instance.GridYToWorldZ(-2 + Mathf.RoundToInt(BuildingGrid.instance.GetGridSize() / 2)) + zOffset;
            Instantiate(monkeyPrefab, new Vector3(world_x, world_y + .7f, world_z), Quaternion.identity);
        }
        
        monkeyCount += num;
        monkeyCountText.text = monkeyCount.ToString();
    }
}
