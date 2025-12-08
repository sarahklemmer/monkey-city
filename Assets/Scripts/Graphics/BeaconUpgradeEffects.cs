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
    
    // Selection indicator
    private GameObject selectionIndicator;
    private Material selectionMaterial;

    void Awake()
    {
        if (beaconLevel1Model != null)
            level1Renderers = beaconLevel1Model.GetComponentsInChildren<Renderer>();
        if (beaconLevel2Model != null)
            level2Renderers = beaconLevel2Model.GetComponentsInChildren<Renderer>();
        if (beaconLevel3Model != null)
            level3Renderers = beaconLevel3Model.GetComponentsInChildren<Renderer>();
        
        UpdateVisibleModel(1);
        SetInitialTargetRenderer();
        CreateSelectionIndicator();
    }
    
    private void CreateSelectionIndicator()
    {
        selectionIndicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        selectionIndicator.name = "SelectionIndicator";
        selectionIndicator.transform.SetParent(this.transform);
        selectionIndicator.transform.localPosition = new Vector3(0, 0.5f, 0);
        selectionIndicator.transform.localScale = new Vector3(3f, 0.005f, 3f);
        
        var collider = selectionIndicator.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
            Debug.Log("[BeaconUpgradeEffects] Destroyed collider");
        }
        Shader shader = Shader.Find("Unlit/Color");
        if (shader == null || shader.name == "Hidden/InternalErrorShader")
        {
            shader = Shader.Find("Sprites/Default");
        }
        if (shader == null || shader.name == "Hidden/InternalErrorShader")
        {
            shader = Shader.Find("UI/Default");
        }
        if (shader == null || shader.name == "Hidden/InternalErrorShader")
        {
            shader = new Material(Shader.Find("Standard")).shader;
        }
        
        Debug.Log($"[BeaconUpgradeEffects] Using shader: {shader.name}");
        
        selectionMaterial = new Material(shader);
        selectionMaterial.color = Color.red;
        
        var renderer = selectionIndicator.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = selectionMaterial;
            renderer.sharedMaterial = null;
            renderer.material = selectionMaterial;
            
            Debug.Log("[BeaconUpgradeEffects] Applied RED material to renderer");
        }
        else
        {
            Debug.LogError("[BeaconUpgradeEffects] No renderer found on selection indicator!");
        }
        
        selectionIndicator.SetActive(false);
        
        Debug.Log("[BeaconUpgradeEffects] Selection indicator created (red, super thin ring)");
    }
    
    public void ShowSelectionIndicator()
    {
        Debug.Log($"[BeaconUpgradeEffects] ShowSelectionIndicator called. Indicator exists: {selectionIndicator != null}");
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(true);
            Debug.Log($"[BeaconUpgradeEffects] Selection indicator SHOWN. IsActive: {selectionIndicator.activeSelf}, Position: {selectionIndicator.transform.position}");
        }
        else
        {
            Debug.LogError("[BeaconUpgradeEffects] Selection indicator is NULL!");
        }
    }
    
    public void HideSelectionIndicator()
    {
        Debug.Log($"[BeaconUpgradeEffects] HideSelectionIndicator called");
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
            Debug.Log("[BeaconUpgradeEffects] Selection indicator HIDDEN");
        }
    }

    private void SetInitialTargetRenderer()
    {
        Renderer[] allRenderers = this.GetComponentsInChildren<Renderer>(true);
        
        Renderer targetRenderer = null;
        foreach (var renderer in allRenderers)
        {
            if (renderer != null && renderer.enabled)
            {
                targetRenderer = renderer;
                break;
            }
        }

        if (targetRenderer == null)
        {
            Debug.LogError("[BeaconUpgradeEffects] No enabled renderer found for initial setup!");
            return;
        }

        var targetRendererField = typeof(BuildingBase).GetField("targetRenderer", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (targetRendererField != null)
        {
            targetRendererField.SetValue(this.GetComponent<BuildingBase>(), targetRenderer);
            Debug.Log($"[BeaconUpgradeEffects] Initial targetRenderer set to: {targetRenderer.gameObject.name}");
        }
        else
        {
            Debug.LogError("[BeaconUpgradeEffects] Could not find targetRenderer field in BuildingBase!");
        }
    }

    void Start()
    {
        ReinitializeOutlineEffect();
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
        
        currentLevel = newLevel;
        UpdateVisibleModel(newLevel);
        ReinitializeOutlineEffect();
    }
    
    private void ReinitializeOutlineEffect()
    {
        Renderer[] allRenderers = this.GetComponentsInChildren<Renderer>(true);
        
        if (allRenderers == null || allRenderers.Length == 0)
        {
            Debug.LogWarning($"[BeaconUpgradeEffects] No renderers found on Beacon GameObject");
            return;
        }

        System.Collections.Generic.List<Renderer> enabledRenderers = new System.Collections.Generic.List<Renderer>();
        foreach (var renderer in allRenderers)
        {
            if (renderer != null && renderer.enabled)
            {
                enabledRenderers.Add(renderer);
            }
        }

        if (enabledRenderers.Count == 0)
        {
            Debug.LogWarning($"[BeaconUpgradeEffects] No enabled renderers found");
            return;
        }

        Renderer targetRenderer = enabledRenderers[0];
        Debug.Log($"[BeaconUpgradeEffects] Using renderer: {targetRenderer.gameObject.name} (from {enabledRenderers.Count} enabled renderers)");

        var targetRendererField = typeof(BuildingBase).GetField("targetRenderer", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (targetRendererField != null)
        {
            targetRendererField.SetValue(this.GetComponent<BuildingBase>(), targetRenderer);
            Debug.Log($"[BeaconUpgradeEffects] Updated targetRenderer to {targetRenderer.gameObject.name}");
        }

        var glowField = typeof(BuildingBase).GetField("glow", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (glowField != null)
        {
            var glow = glowField.GetValue(this.GetComponent<BuildingBase>()) as GlowEffect;
            if (glow != null)
            {
                var outlineMaterialField = typeof(BuildingBase).GetField("outlineMaterial", 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance);
                
                if (outlineMaterialField != null)
                {
                    var outlineMaterial = outlineMaterialField.GetValue(this.GetComponent<BuildingBase>()) as Material;
                    if (outlineMaterial != null)
                    {
                        glow.Initialize(outlineMaterial, targetRenderer);
                        Debug.Log($"[BeaconUpgradeEffects] Reinitialized GlowEffect - outlining entire Beacon");
                    }
                }
            }
        }
    }

    private void UpdateVisibleModel(int level)
    {
        SetRenderersEnabled(level1Renderers, false);
        SetRenderersEnabled(level2Renderers, false);
        SetRenderersEnabled(level3Renderers, false);
        
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