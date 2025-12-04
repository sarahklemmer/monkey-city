using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BananaGrow : MonoBehaviour
{
    [SerializeField] List<GameObject> nanners;
    [SerializeField] GameObject treetop;

    void Start()
    {
        Assert.IsNotNull(nanners, "bananas is null D:");
        Assert.IsNotNull(treetop, "didnt assign treetop in prefab inspector D:");
        Assert.AreNotEqual(nanners.Count, 0, "no bananas D:");  
    }

    public void Grow()
    {
        foreach(GameObject g in nanners) { g.transform.localScale *= 1.75f; }
    }

    public void GrowTreetop()
    {
        treetop.transform.localScale = new Vector3(1.4f, 1, 1.3f);
    }
}
