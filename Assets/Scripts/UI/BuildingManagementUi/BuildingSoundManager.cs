using UnityEngine;

public class BuildingSoundManager : MonoBehaviour
{
    public static BuildingSoundManager instance;
    
    [Header("Building Sounds")]
    [SerializeField] private AudioClip buildingPlacedSound;
    [SerializeField] private AudioClip upgradeSound;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] [Range(0f, 1f)] private float buildVolume = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float upgradeVolume = 0.8f;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound
        }
    }
    
    public void PlayBuildingPlacedSound()
    {
        if (buildingPlacedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buildingPlacedSound, buildVolume);
            Debug.Log("[BuildingSoundManager] Playing building placed sound");
        }
        else
        {
            Debug.LogWarning("[BuildingSoundManager] Building placed sound or AudioSource is null!");
        }
    }
    
    public void PlayUpgradeSound()
    {
        if (upgradeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(upgradeSound, upgradeVolume);
            Debug.Log("[BuildingSoundManager] Playing upgrade sound");
        }
        else
        {
            Debug.LogWarning("[BuildingSoundManager] Upgrade sound or AudioSource is null!");
        }
    }
    
    public void PlayBuildingSoundAt(Vector3 position)
    {
        if (buildingPlacedSound != null)
        {
            AudioSource.PlayClipAtPoint(buildingPlacedSound, position, buildVolume);
        }
    }
    
    public void PlayUpgradeSoundAt(Vector3 position)
    {
        if (upgradeSound != null)
        {
            AudioSource.PlayClipAtPoint(upgradeSound, position, upgradeVolume);
        }
    }
}