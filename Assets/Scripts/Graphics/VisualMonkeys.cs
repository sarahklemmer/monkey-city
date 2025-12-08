using UnityEngine;
using UnityEngine.Assertions;

public class VisualMonkeys : MonoBehaviour
{
    [SerializeField] private GameObject monkey1;
    [SerializeField] private GameObject monkey2;
    [SerializeField] private GameObject[] extraMonkeys; // additional monkey visuals beyond the first two

    private Renderer[] rs1;
    private Renderer[] rs2;
    private Renderer[][] extraRenderers;

    void Awake() { 
        Assert.IsNotNull(monkey1, "no visual monkeys assigned in " + gameObject.name);
        rs1 = monkey1.GetComponentsInChildren<Renderer>();
        if(monkey2 != null) rs2 = monkey2.GetComponentsInChildren<Renderer>();

        if (extraMonkeys != null && extraMonkeys.Length > 0)
        {
            extraRenderers = new Renderer[extraMonkeys.Length][];
            for (int i = 0; i < extraMonkeys.Length; i++)
            {
                GameObject extra = extraMonkeys[i];
                extraRenderers[i] = extra != null ? extra.GetComponentsInChildren<Renderer>() : null;
            }
        }
        else
        {
            extraRenderers = new Renderer[0][];
        }
    }

    void Start()
    {
        SetMonkeyCount(0); 
    } 

    public void SetMonkeyCount(int count)
    {  
        int maxCount = 2 + (extraMonkeys?.Length ?? 0);
        Assert.IsTrue(0 <= count && count <= maxCount, "setting monkey count too high in visualmonkeys!");
        SetEnabled(rs1, count >= 1);
        if(monkey2 != null) SetEnabled(rs2, count >= 2);
        
        int remaining = count - 2;
        if (remaining > 0)
        {
            for (int i = 0; i < extraRenderers.Length; i++)
            {
                SetEnabled(extraRenderers[i], remaining > i);
            }
        }
        else if (extraRenderers.Length > 0)
        {
            for (int i = 0; i < extraRenderers.Length; i++)
            {
                SetEnabled(extraRenderers[i], false);
            }
        }
    }

    private static void SetEnabled(Renderer[] rs, bool enabled) 
    { 
        if (rs == null) return;
        foreach (var r in rs) r.enabled = enabled; 
    }
}
