using UnityEngine;

public class BeaconUpgradeEffects : MonoBehaviour
{
    [Header("Beacon Models - Use Renderers or MeshRenderers")]
    [Tooltip("Assign the GameObjects that contain the visual meshes")]
    [SerializeField] private GameObject beaconLevel1Model;
    [SerializeField] private GameObject beaconLevel2Model;
    [SerializeField] private GameObject beaconLevel3Model;
    
    private int currentLevel = 1;
    
    // Cache renderers for performance
    private Renderer[] level1Renderers;
    private Renderer[] level2Renderers;
    private Renderer[] level3Renderers;

    void Awake()
    {
        // Cache all renderers from each model
        if (beaconLevel1Model != null)
            level1Renderers = beaconLevel1Model.GetComponentsInChildren<Renderer>();
        if (beaconLevel2Model != null)
            level2Renderers = beaconLevel2Model.GetComponentsInChildren<Renderer>();
        if (beaconLevel3Model != null)
            level3Renderers = beaconLevel3Model.GetComponentsInChildren<Renderer>();
        
        // Initialize in Awake to ensure it happens before Beacon's Start()
        UpdateVisibleModel(1);
    }

    public void Upgrade(int newLevel)
    {
        if (newLevel == currentLevel) 
        {
            Debug.LogWarning($"[BeaconUpgradeEffects] Attempted to upgrade to same level: {newLevel}");
            return;
        }
        
        if (newLevel < 1 || newLevel > 3)
        {
            Debug.LogError($"[BeaconUpgradeEffects] Invalid level: {newLevel}. Must be between 1 and 3.");
            return;
        }
        
        Debug.Log($"[BeaconUpgradeEffects] Upgrading visual from level {currentLevel} to {newLevel}");
        currentLevel = newLevel;
        UpdateVisibleModel(newLevel);
    }

    private void UpdateVisibleModel(int level)
    {
        // Hide all models by disabling their renderers (NOT SetActive!)
        SetRenderersEnabled(level1Renderers, false);
        SetRenderersEnabled(level2Renderers, false);
        SetRenderersEnabled(level3Renderers, false);
        
        // Show the appropriate model's renderers
        switch (level)
        {
            case 1:
                if (level1Renderers != null && level1Renderers.Length > 0) 
                {
                    SetRenderersEnabled(level1Renderers, true);
                    Debug.Log($"[BeaconUpgradeEffects] Level 1 model shown ({level1Renderers.Length} renderers)");
                }
                else
                {
                    Debug.LogWarning("[BeaconUpgradeEffects] Level 1 model has no renderers!");
                }
                break;
            case 2:
                if (level2Renderers != null && level2Renderers.Length > 0) 
                {
                    SetRenderersEnabled(level2Renderers, true);
                    Debug.Log($"[BeaconUpgradeEffects] Level 2 model shown ({level2Renderers.Length} renderers)");
                }
                else
                {
                    Debug.LogWarning("[BeaconUpgradeEffects] Level 2 model has no renderers!");
                }
                break;
            case 3:
                if (level3Renderers != null && level3Renderers.Length > 0) 
                {
                    SetRenderersEnabled(level3Renderers, true);
                    Debug.Log($"[BeaconUpgradeEffects] Level 3 model shown ({level3Renderers.Length} renderers)");
                }
                else
                {
                    Debug.LogWarning("[BeaconUpgradeEffects] Level 3 model has no renderers!");
                }
                break;
            default:
                Debug.LogError($"[BeaconUpgradeEffects] Invalid level in UpdateVisibleModel: {level}");
                break;
        }
    }
    
    private void SetRenderersEnabled(Renderer[] renderers, bool enabled)
    {
        if (renderers == null) 
        {
            Debug.LogWarning("[BeaconUpgradeEffects] Renderers array is null!");
            return;
        }
        
        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                Debug.Log($"[BeaconUpgradeEffects] Setting {renderer.gameObject.name}.{renderer.GetType().Name}.enabled = {enabled}");
                renderer.enabled = enabled;
                
                // Double-check it actually worked
                if (renderer.enabled != enabled)
                {
                    Debug.LogError($"[BeaconUpgradeEffects] FAILED to set {renderer.gameObject.name} enabled to {enabled}!");
                }
            }
        }
    }

    void OnValidate()
    {
        if (beaconLevel1Model == null)
            Debug.LogWarning("[BeaconUpgradeEffects] Level 1 model is not assigned in the inspector!");
        if (beaconLevel2Model == null)
            Debug.LogWarning("[BeaconUpgradeEffects] Level 2 model is not assigned in the inspector!");
        if (beaconLevel3Model == null)
            Debug.LogWarning("[BeaconUpgradeEffects] Level 3 model is not assigned in the inspector!");
    }
}