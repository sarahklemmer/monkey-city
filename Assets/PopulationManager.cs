using UnityEngine;
using TMPro;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private GameObject monkeyPrefab;
    [SerializeField] private TextMeshProUGUI monkeyCountText;
    [SerializeField] private int monkeyCount = 0;
    void Start()
    {
        monkeyCount = 0;
        UpdateMonkeyCountText();
    }

    public void AddBananas(int num)
    {
        monkeyCount += num;
        Debug.Log("Banana(s) added! Total monkeys: " + monkeyCount);
        UpdateMonkeyCountText();
    }

    public void SpendBananas(int num){
        if(monkeyCount >= num){
            monkeyCount -= num;
            Debug.Log("Monkey(s) died! Total monkeys: " + monkeyCount);
            UpdateMonkeyCountText();
        } else {
            Debug.Log("ERROR: NOT ENOUGH MONKEYS TO DIE!");
        }
    }

    private void UpdateMonkeyCountText()
    {
        if (monkeyCountText != null)
        {
            monkeyCountText.text = monkeyCount.ToString();
        }
    }
}
