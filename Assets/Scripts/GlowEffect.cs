using UnityEngine;

public class GlowEffect : MonoBehaviour
{
    
    private Renderer r;
    private Material[] baseMaterials;
    private Material[] outlinedMaterials;
    
    public void Initialize(Material outlineMaterial)
    {
        r = GetComponent<Renderer>();

        // Save base materials once
        baseMaterials = r.sharedMaterials;

        // Make an outlined version of that array
        outlinedMaterials = new Material[baseMaterials.Length + 1];
        baseMaterials.CopyTo(outlinedMaterials, 0);
        outlinedMaterials[^1] = outlineMaterial;
    }

    public void SetGlow(bool on)
    {
        r.materials = on ? outlinedMaterials : baseMaterials;
    }
}
