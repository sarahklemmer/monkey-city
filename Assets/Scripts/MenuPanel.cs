using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject menuPanel;
    void Start()
    {
        menuPanel = this.gameObject;
        menuPanel.SetActive(false);
    }

    // Update is called once per frame
    public void HideMenu()
    {
        menuPanel.SetActive(false);
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
    }
}
