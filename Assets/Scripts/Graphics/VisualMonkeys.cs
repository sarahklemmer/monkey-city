using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class VisualMonkeys : MonoBehaviour
{
    [SerializeField] private GameObject monkey1;
    [SerializeField] private GameObject monkey2;

    private Renderer[] rs1;
    private Renderer[] rs2;

    void Awake() { 
        Assert.IsNotNull(monkey1, "monkey 1 not assigned in " + gameObject.name);
        Assert.IsNotNull(monkey2, "monkey 2 not assigned in " + gameObject.name);
        rs1 = monkey1.GetComponentsInChildren<Renderer>();
        rs2 = monkey2.GetComponentsInChildren<Renderer>();
    }

    void Start()
    {
        SetMonkeyCount(0); 
    } 

    public void SetMonkeyCount(int count)
    {  
        Assert.IsTrue(0 <= count && count <= 2, "setting monkey count too high in visualmonkeys!");
        SetEnabled(rs1, count >= 1);
        SetEnabled(rs2, count >= 2);
    }

    private static void SetEnabled(Renderer[] rs, bool enabled) { foreach (var r in rs) r.enabled = enabled; }
}
