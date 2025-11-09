using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Renderer targetRenderer;
    private GlowEffect glow;

    protected Building building;
    protected BuildingMonkeys monkeys;
    protected Renderer[] renderers;
    protected Collider[] colliders;
    public bool selectable{ get; private set; }

    // functions to be overrode
    public abstract void OnDayCycle();
    public abstract void OnDestroy();

    void Update()
    {
        UpdateBehavior();
    }

    // basically RAII for the BuildingManager, super handy
    protected virtual void OnEnable()
    {
        BuildingManager.instance.AddBuilding(this);
    }

    protected virtual void OnDisable()
    {
        BuildingManager.instance.RemoveBuilding(this);
    }

    protected virtual void SharedAwakeBehavior()
    {
        glow = gameObject.AddComponent<GlowEffect>();
        glow.Initialize(outlineMaterial, targetRenderer);
        // TODO: make this dynamic, placeholder of 2 for now
        monkeys = new(2);
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        selectable = true;
    }

    protected virtual void UpdateBehavior()
    {

    }

    public virtual bool RemoveMonkey(MonkeyController m)
    {
        return monkeys.Remove(m);
    }

    public virtual void RemoveNextMonkey()
    {
        monkeys.Remove(NextMonkeyToRemove());
    }

    public virtual bool AddMonkey(MonkeyController monkey)
    {
        // returns whether the add was succeeded
        return monkeys.Add(monkey);
    }
    
    public virtual MonkeyController NextMonkeyToRemove()
    {
        return monkeys.MonkeyToDeallocate();
    }

    // From clickBuilding branch - used by BuildingInfo to show panel
    void OnMouseDown()
    {
        // Use BuildingSelector instead of directly showing BuildingInfo
        BuildingSelector.instance.Select(this);
    }

    // From clickBuilding branch - glow methods now use GlowEffect from develop
    public void EnableGlow()
    {
        glow.SetGlow(true);
    }

    public void DisableGlow()
    {
        glow.SetGlow(false);
    }

    // From clickBuilding branch - helper methods for BuildingInfo display
    public virtual int GetMonkeyCount()
    {
        return monkeys?.count ?? 0;
    }

    public virtual int GetMonkeyCapacity()
    {
        return monkeys?.capacity ?? 0;
    }

    public BuildingType GetBuildingType() => building.type;

    public string GetMonkeyAllocString()
    {
        return monkeys.Count().ToString() + "/" + monkeys.Capacity() + " monkeys";
    }
    //START OF AI CODE
    public void MakeTransparent()
    {
        if (renderers == null) return;
        selectable = false;
        foreach (var c in colliders) c.enabled = false;

        foreach (var r in renderers)
        {
            var mats = r.materials; // per-instance
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];

                // Set alpha (URP uses _BaseColor, built-in uses _Color)
                if (m.HasProperty("_BaseColor"))
                {
                    var c = m.GetColor("_BaseColor");
                    c.a = 0.2f;
                    m.SetColor("_BaseColor", c);
                }
                else if (m.HasProperty("_Color"))
                {
                    var c = m.color;
                    c.a = 0.2f;
                    m.color = c;
                }

                // URP surface toggle if available
                if (m.HasProperty("_Surface")) m.SetFloat("_Surface", 1f); // Transparent

                // Built-in Standard fallback settings
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                m.SetInt("_ZWrite", 0);
                m.DisableKeyword("_ALPHATEST_ON");
                m.EnableKeyword("_ALPHABLEND_ON");
                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
        }


    }

    public void MakeOpaque()
    {
        if (renderers == null) return;
        selectable = true;
        foreach (var c in colliders) c.enabled = true;

        foreach (var r in renderers)
        {
            var mats = r.materials; // per-instance
            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];

                // Restore alpha
                if (m.HasProperty("_BaseColor"))
                {
                    var c = m.GetColor("_BaseColor");
                    c.a = 1f;
                    m.SetColor("_BaseColor", c);
                }
                else if (m.HasProperty("_Color"))
                {
                    var c = m.color;
                    c.a = 1f;
                    m.color = c;
                }

                // URP surface toggle if available
                if (m.HasProperty("_Surface")) m.SetFloat("_Surface", 0f); // Opaque

                // Built-in Standard fallback settings
                m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                m.SetInt("_ZWrite", 1);
                m.DisableKeyword("_ALPHATEST_ON");
                m.DisableKeyword("_ALPHABLEND_ON");
                m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                m.renderQueue = -1; // use shader default
            }
        }
    }
    //END OF AI CODE
}