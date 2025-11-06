using UnityEngine;
using TMPro;

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


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Duplicate BananaManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        bananaCount = 0;
        UpdateBananaCountText();
    }

    public void AddBananas(int num)
    {
        bananaCount += num;
        UpdateBananaCountText();
        Debug.Log("Banana(s) added! Total bananas: " + bananaCount);
    }

    public void SpendBananas(int num){
        if(bananaCount >= num){
            bananaCount -= num;
            Debug.Log("Banana(s) spent! Total bananas: " + bananaCount);
            UpdateBananaCountText();
        } else {
            Debug.Log("Not enough bananas to spend!");
        }
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
