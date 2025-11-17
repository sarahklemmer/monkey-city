using TMPro;
using UnityEngine;

public class ResourceBar : MonoBehaviour
{
    [SerializeField] TMP_Text bananaCount;
    [SerializeField] TMP_Text bananaProduction;
    [SerializeField] TMP_Text populationCount;

    void Update()
    {
        bananaCount.text = BananaManager.instance.bananaCount.ToString();
        int production = BuildingManager.instance.GetBananaProduction();
        // if negative it'll have the '-' in the string, otherwise have to add a plus
        bananaProduction.text = production < 0 ? production.ToString() : "+" + production;
        bananaProduction.color = production == 0 ? Color.black : (production < 0 ? Color.red : Color.green);

        populationCount.text = PopulationManager.instance.population.ToString();
    }
}
