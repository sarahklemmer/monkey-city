using UnityEngine;

public class GlowEffect : MonoBehaviour
{
    private Renderer r;
    private Material[] baseMaterials;
    private Material outlineMaterial;
    private bool isGlowing = false;

    public void Initialize(Material outlineMaterial, Renderer target = null)
    {
        r = target == null ? GetComponent<Renderer>() : target;
        this.outlineMaterial = outlineMaterial;

        // initial "base" state
        baseMaterials = r.sharedMaterials;
    }

    public void SetGlow(bool on)
    {
        if (r == null) return;

        if (on)
        {
            if (isGlowing) return; // already on

            // Take whatever the renderer currently has as the new base
            baseMaterials = r.sharedMaterials;

            var outlined = new Material[baseMaterials.Length + 1];
            baseMaterials.CopyTo(outlined, 0);
            outlined[outlined.Length - 1] = outlineMaterial;

            // Use sharedMaterials so we stay consistent with how we read them
            r.sharedMaterials = outlined;
            isGlowing = true;
        }
        else
        {
            if (!isGlowing) return; // already off

            // Current materials include the outline; strip it off,
            // but preserve whatever changes were made to the base slots.
            var current = r.sharedMaterials;
            if (current.Length > 0)
            {
                int newLen = current.Length - 1;
                var newBase = new Material[newLen];
                System.Array.Copy(current, newBase, newLen);

                baseMaterials = newBase;
                r.sharedMaterials = baseMaterials;
            }
            else
            {
                // fallback: just restore whatever we last knew as base
                r.sharedMaterials = baseMaterials;
            }

            isGlowing = false;
        }
    }
}