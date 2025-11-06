using UnityEngine;
using TMPro;

public class HappinessManager : MonoBehaviour
{
    [SerializeField] private GameObject happinessPrefab;
    [SerializeField] private TextMeshProUGUI happinessCountText;
    [SerializeField] private int happinessCount = 50;
    void Start()
    {
        happinessCount = 50;
        UpdateHappinessCountText();
    }

    public void IncreaseHappiness(int num)
    {
        if (happinessCount + num > 100) {
            happinessCount = 100;
        } else {    
            happinessCount += num;
        }
        Debug.Log("Happiness(s) added! Total happinesss: " + happinessCount);
        UpdateHappinessCountText();
    }

    public void DecreaseHappiness(int num){
        if(happinessCount >= num){
            happinessCount -= num;
        } else {
            happinessCount = 0;
        }
        UpdateHappinessCountText();

    }

    private void UpdateHappinessCountText()
    {
        if (happinessCountText != null)
        {
            happinessCountText.text = happinessCount.ToString();
        }
    }
}
