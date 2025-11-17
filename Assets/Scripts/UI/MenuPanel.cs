using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject menuPanel;
    private bool isVisible = false;
    void Start()
    {
        menuPanel = gameObject;
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

    public void ToggleMenu()
    {
        isVisible = !isVisible;
        menuPanel.SetActive(isVisible);
    }
}
