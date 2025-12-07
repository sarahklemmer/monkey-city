using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Button muteButton;
    public static bool isMuted = false;

    void Start()
    {
        // muteButton.onClick.AddListener(ToggleMute);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        AudioSource[] allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.mute = isMuted;
        }
        VolumeButton.instance.ToggleVolume();
        Debug.Log("Toggled");
    }

    public static bool IsMuted(bool dummy)
    {
        dummy = isMuted;
        return dummy;
    }
}