using UnityEngine;
using UnityEngine.UI;

public class BuildingInfo : MonoBehaviour
{
    [SerializeField] GameObject removeButton;
    [SerializeField] GameObject info;

    public static BuildingInfo instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BuildingInfo on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void Show(BuildingBase building)
    {
        //TODO: REMOVE ME
        return;

        info.SetActive(true);
        // display info using whatever components however you'd like :D
        removeButton.GetComponent<Button>().onClick.AddListener(building.Remove);
    }

    public void Hide()
    {
        //TODO: REMOVE ME
        return;
        
        removeButton.SetActive(false);
        info.SetActive(false);
    }
}
