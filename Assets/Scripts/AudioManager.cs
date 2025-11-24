using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Button muteButton;
    private bool isMuted = false;

    void Start()
    {
        muteButton.onClick.AddListener(ToggleMute);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.mute = isMuted;
        }
    }
}