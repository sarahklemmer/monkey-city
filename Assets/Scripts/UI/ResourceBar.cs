using TMPro;
using UnityEngine;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] TMP_Text bananaCount;
    [SerializeField] TMP_Text playerPointCount;
    [SerializeField] TMP_Text bananaProduction;
    [SerializeField] TMP_Text populationCount;
    BuildingBase treeOfLife;

    void Start()
    {
        populationCount.text = "";
    }

    void Update()
    {
        bananaCount.text = BananaManager.instance.bananaCount.ToString();
        playerPointCount.text = PathManager.instance.playerPoints.ToString();
        int production = BuildingManager.instance.GetBananaProduction();
        // if negative it'll have the '-' in the string, otherwise have to add a plus
        bananaProduction.text = production < 0 ? production.ToString() : "+" + production;
        bananaProduction.color = production == 0 ? Color.black : (production < 0 ? Color.red : Color.green);
        if(treeOfLife == null) treeOfLife = BuildingManager.instance.GetTreeOfLife();
        else populationCount.text = treeOfLife.GetMonkeyCount().ToString();
    }
}
