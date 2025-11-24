using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public bool paused { get; private set; } = false;
    bool stoppedToggleFlashing;
    [SerializeField] Image icon;
    [SerializeField] Sprite pause; 
    [SerializeField] Sprite play;
    [SerializeField] GameObject pausePanel;

    void Update()
    {
        icon.sprite = paused ? play : pause;
    }

    public void Click()
    {
        if(paused) Unpause();
        else Pause();
    }

    void Pause()
    {
        TimeController.instance.StopTicking();
        BananaProductionTimer.instance.StopProduction();
        WaveSpawner.instance.PauseSpawning();
        UIInteractabilityManager.instance.DisableInteractivity();
        
        pausePanel.SetActive(true);
        paused = true;

        stoppedToggleFlashing = ToggleFlasher.instance.active;
        ToggleFlasher.instance.StopFlash();
    }

    void Unpause()
    {
        TimeController.instance.StopTicking();
        BananaProductionTimer.instance.StopProduction();
        WaveSpawner.instance.PauseSpawning();
        UIInteractabilityManager.instance.EnableInteractivity();

        pausePanel.SetActive(false);
        paused = false;

        if(stoppedToggleFlashing) ToggleFlasher.instance.StartFlash();
    }
}
