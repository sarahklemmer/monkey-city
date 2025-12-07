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

    public bool pathSelected = false;

    [SerializeField] private float discountFactor = 0.5f;
    [SerializeField] public int playerPoints = 0;

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

    public void FarmerPathUpgrade()
    {
        int costOfUpgrade = CalculateCostOfUpgrade(PathType.Farmer);
        if (playerPoints < costOfUpgrade)
        {
            ToastManager.Instance?.ReplaceToast("You don't have enough player points for the Farmer path upgrade", 1f);
            return;
        }

        playerPoints -= costOfUpgrade;
        farmerPathLevel++;

        IncreaseFarmerPathLevel();
    }

    public void WarriorPathUpgrade()
    {
        int costOfUpgrade = CalculateCostOfUpgrade(PathType.Warrior);
        if (playerPoints < costOfUpgrade)
        {
            ToastManager.Instance?.ReplaceToast("You don't have enough player points for the Warrior path upgrade", 1f);
            return;
        }

        playerPoints -= costOfUpgrade;
        warriorPathLevel++;

        IncreaseWarriorPathLevel();
    }

    public void ScholarPathUpgrade()
    {
        int costOfUpgrade = CalculateCostOfUpgrade(PathType.Scholar);
        if (playerPoints < costOfUpgrade)
        {
            ToastManager.Instance?.ReplaceToast("You don't have enough player points for the Scholar path upgrade", 1f);
            return;
        }

        playerPoints -= costOfUpgrade;
        scholarPathLevel++;

        IncreaseScholarPathLevel();
    }
    
    public int CalculateCostOfUpgrade(PathType pathType)
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
        // if (farmerPathLevel % 2 == 1)
        // {
        AllBananaFarmInfo.instance.IncreaseBananasPerDay(1);
        ToastManager.Instance?.RequestToast("Farmer path upgraded to level " + farmerPathLevel + "!", 1f);
        // }
        // else
        // {
        //     //AllBananaFarmInfo.instance.IncreaseBananasPerDay(2);
        // }
    }
    
    void IncreaseWarriorPathLevel()
    {
        // Debug.Log($"Warrior path upgraded to level {warriorPathLevel}!");
        // if (warriorPathLevel % 2 == 1)
        // {
        AllArcherTowerInfo.instance.IncreaseDamagePerAttack(5);
        ToastManager.Instance?.RequestToast("Warrior path upgraded to level " + warriorPathLevel + "!", 1f);
        // }
        // else
        // {
        //     //AllArcherTowerInfo.instance.IncreaseDamagePerAttack(2);
        // }
    }
    
    void IncreaseScholarPathLevel()
    {
        // Debug.Log($"Scholar path upgraded to level {scholarPathLevel}!");
        // if (scholarPathLevel % 2 == 1)
        // {
        AllMonkeyInfo.instance.IncreaseMonkeySpeed(2f);
        ToastManager.Instance?.RequestToast("Scholar path upgraded to level " + scholarPathLevel + "!", 1f);
        // }
        // else
        // {
        //     //AllEnemyInfo.instance.IncreaseMaxHealth(20);
        // }
    }

    // Public methods to select initial path (call from UI buttons)
    public void SelectFarmerPath()
    {
        currentPathType = PathType.Farmer;
        pathSelected = true;
        ToastManager.Instance?.RequestToast("Farmer path selected!", 2f);
    }

    public void SelectWarriorPath()
    {
        currentPathType = PathType.Warrior;
        pathSelected = true;
        ToastManager.Instance?.RequestToast("Warrior path selected!", 2f);
    }

    public void SelectScholarPath()
    {
        currentPathType = PathType.Scholar;
        pathSelected = true;
        ToastManager.Instance?.RequestToast("Scholar path selected!", 2f);
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

