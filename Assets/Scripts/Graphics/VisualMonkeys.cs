using UnityEngine;
using UnityEngine.Assertions;

public class VisualMonkeys : MonoBehaviour
{
    [SerializeField] private GameObject monkey1;
    [SerializeField] private GameObject monkey2;

    private Renderer[] rs1;
    private Renderer[] rs2;

    [SerializeField] private bool tutorialMode = false;

    void Awake() {
        if (tutorialMode) return;
        Assert.IsNotNull(monkey1, "no visual monkeys assigned in " + gameObject.name);
        rs1 = monkey1.GetComponentsInChildren<Renderer>();
        if(monkey2 != null) rs2 = monkey2.GetComponentsInChildren<Renderer>();
    }

    void Start()
    {
        if (tutorialMode) return;
        SetMonkeyCount(0); 
    } 

    public void SetMonkeyCount(int count)
    {  
        if (tutorialMode) return;
        Assert.IsTrue(0 <= count && count <= 2, "setting monkey count too high in visualmonkeys!");
        SetEnabled(rs1, count >= 1);
        if(monkey2 != null) SetEnabled(rs2, count >= 2);
    }

    private static void SetEnabled(Renderer[] rs, bool enabled) { foreach (var r in rs) r.enabled = enabled; }
}
