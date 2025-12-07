using UnityEngine;

public class TreeOfLifeUpgradeEffects : MonoBehaviour
{
    [SerializeField] Color bloomColor;

    public void Upgrade()
    {
        var r = GetComponent<Renderer>();
        var mats = r.materials;
        mats[1].name = "treecolor";
        mats[1].color = bloomColor;
        r.materials = mats;
    }
}