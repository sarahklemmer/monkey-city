using UnityEngine;

public class MonkeyOnClick : MonoBehaviour
{
    
    // this is from chat, it's for the outline effect
    [SerializeField] private Material outlineMaterial;
    private Renderer r;
    private Material[] baseMaterials;
    private Material[] outlinedMaterials;
    private bool glowing = false;

    void Awake()
    {
        r = GetComponent<Renderer>();

        // Save base materials once
        baseMaterials = r.sharedMaterials;

        // Make an outlined version of that array
        outlinedMaterials = new Material[baseMaterials.Length + 1];
        baseMaterials.CopyTo(outlinedMaterials, 0);
        outlinedMaterials[^1] = outlineMaterial;
    }
    
    public void ToggleHighlight()
    {
        glowing = !glowing;
        r.materials = glowing ? outlinedMaterials : baseMaterials;
    }

    void OnMouseDown()
    {
        ToggleHighlight();
    }
}
