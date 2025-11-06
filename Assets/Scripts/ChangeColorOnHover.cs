using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Renderer), typeof(Collider))]
public class ChangeColorOnHover : MonoBehaviour
{
    Renderer r;
    Material originalMaterial;
    [SerializeField] Material onHoverMaterial;

    void Awake()
    {
        r = GetComponent<Renderer>();
        Assert.IsNotNull(r, "attaching ChangeColorOnHover to a component without a renderer!");
        originalMaterial = r.material;
        Assert.IsNotNull(originalMaterial, "couldn't find material on renderer!");
        Assert.IsNotNull(onHoverMaterial, "no material provided for hovering!");
    }

    void OnMouseEnter()
    {
        r.material = onHoverMaterial;
    }

    void OnMouseExit()
    {
        r.material = originalMaterial;
    }
}
