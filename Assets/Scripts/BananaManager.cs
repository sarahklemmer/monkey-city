using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

public class BananaManager : MonoBehaviour
{
    [SerializeField] private GameObject bananaPrefab;
    [SerializeField] private TextMeshProUGUI bananaCountText;
    [SerializeField] private int bananaCount = 0;

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
        UpdateBananaCountText();
    }

    public void AddBananas(int num)
    {
        bananaCount += num;
        UpdateBananaCountText();
    }

    public void RemoveBananas(int num)
    {
        Assert.IsTrue(bananaCount <= num, "removing more bananas than we have!");
        bananaCount -= num;
        UpdateBananaCountText();
    }

    public int GetBananas() => bananaCount;

    private void UpdateBananaCountText()
    {
        if (bananaCountText != null)
        {
            bananaCountText.text = bananaCount.ToString();
        }
    }
}
