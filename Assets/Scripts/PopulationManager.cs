using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager instance;
    
    [SerializeField] private GameObject monkeyPrefab;
    [SerializeField] private TextMeshProUGUI monkeyCountText;
    [SerializeField] private int monkeyCount = 0;
    
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

        if (monkeyCount + num < 0)
        {
            //Call end game function
        }
        
        monkeyCount += num;
        monkeyCountText.text = monkeyCount.ToString();
    }
}
