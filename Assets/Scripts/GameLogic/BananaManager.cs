using UnityEngine;
using UnityEngine.Assertions;

public class BananaManager : MonoBehaviour
{
    [SerializeField] private int startingBananas = 1;
    public int bananaCount {get; private set;}
    private int bananasGenerated = 0;

    public static BananaManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BananaManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        AddBananas(startingBananas);
    }

    public void AddBananas(int num)
    {
        bananaCount += num;
        bananasGenerated += num;
        Debug.Log($"Added {num} bananas. Total bananas: {bananaCount}");
    }

    public void RemoveBananas(int num)
    {
        Assert.IsTrue(bananaCount >= num, "removing more bananas than we have!");
        bananaCount -= num;
    }

    public int GetBananas() => bananaCount;
    public int GetBananasGenerated() => bananasGenerated;
}
