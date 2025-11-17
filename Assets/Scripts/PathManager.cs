using UnityEngine;

public enum PathType
{
    Farmer,
    Warrior,
    Scholar
}

public class PathManager : MonoBehaviour
{
    public static PathManager instance;

    private int farmerPathLevel = 0;
    private int warriorPathLevel = 0;
    private int scholarPathLevel = 0;

    [SerializeField] private int[] levelCosts = { 2, 2, 3, 5, 8, 10};

    public PathType currentPathType = PathType.Farmer;

    [SerializeField] private float discountFactor = 0.5f;
    [SerializeField] private int playerPoints = 0;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate PathManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    void Start()
    {
        playerPoints = 0;
        farmerPathLevel = 0;
        warriorPathLevel = 0;
        scholarPathLevel = 0;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            FarmerPathUpgrade();
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            WarriorPathUpgrade();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            ScholarPathUpgrade();
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            playerPoints++;
        }
    }

    void FarmerPathUpgrade()
    {
        int costOfUpgrade = CalculateCostOfUpgrade(PathType.Farmer);
        if (playerPoints < costOfUpgrade){ return; }

        playerPoints -= costOfUpgrade;
        farmerPathLevel++;

        IncreaseFarmerPathLevel();
    }

    void WarriorPathUpgrade()
    {
        int costOfUpgrade = CalculateCostOfUpgrade(PathType.Warrior);
        if (playerPoints < costOfUpgrade){ return; }

        playerPoints -= costOfUpgrade;
        warriorPathLevel++;

        IncreaseWarriorPathLevel();
    }

    void ScholarPathUpgrade()
    {
        int costOfUpgrade = CalculateCostOfUpgrade(PathType.Scholar);
        if (playerPoints < costOfUpgrade){ return; }

        playerPoints -= costOfUpgrade;
        scholarPathLevel++;

        IncreaseScholarPathLevel();
    }
    
    int CalculateCostOfUpgrade(PathType pathType)
    {
        int costOfUpgrade = levelCosts[GetPathLevel(pathType)];
        
        if (currentPathType == pathType)
        {
            costOfUpgrade = Mathf.CeilToInt(costOfUpgrade * discountFactor);
        }

        return costOfUpgrade;
    }

    void IncreaseFarmerPathLevel()
    {
        AllBananaFarmInfo.instance.IncreaseBananasPerDay(1);
    }
    
    void IncreaseWarriorPathLevel()
    {
        Debug.Log($"Warrior path upgraded to level {warriorPathLevel}!");
    }
    
    void IncreaseScholarPathLevel()
    {
        Debug.Log($"Scholar path upgraded to level {scholarPathLevel}!");
    }

    // Public methods to select initial path (call from UI buttons)
    public void SelectFarmerPath()
    {
        currentPathType = PathType.Farmer;
    }

    public void SelectWarriorPath()
    {
        currentPathType = PathType.Warrior;
    }

    public void SelectScholarPath()
    {
        currentPathType = PathType.Scholar;
    }
    
    public int GetPlayerPoints() => playerPoints;

    public void AddPoints(int amount) 
    { 
        playerPoints += amount;
    }

    public int GetPathLevel(PathType pathType)
    {
        switch (pathType)
        {
            case PathType.Farmer:
                return farmerPathLevel;
            case PathType.Warrior:
                return warriorPathLevel;
            case PathType.Scholar:
                return scholarPathLevel;
            default:
                return 0;
        }
    }
    
}

