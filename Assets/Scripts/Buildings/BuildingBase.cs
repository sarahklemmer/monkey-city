using UnityEngine;
using UnityEngine.Assertions;

public abstract class BuildingBase : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Renderer targetRenderer;
    private GlowEffect glow;
    protected Building building;
    protected BuildingMonkeys monkeys;
    protected Renderer[] renderers;
    protected Collider[] colliders;
    public bool selectable { get; private set; }
    public int bananasPerDay { get; protected set; } = 0;
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
        ToggleFlasher.instance.StopFlash();
    }

    protected virtual void OnDisable()
    {
        BuildingManager.instance.RemoveBuilding(this);
        BuildingGrid.instance.RemoveBuilding(building);
    }

    protected virtual void SharedAwakeBehavior()
    {
        Assert.IsTrue(
            gameObject.layer == LayerMask.NameToLayer("Building"),
            $"{gameObject.name} MUST be on the 'Building' layer, but is currently on '{LayerMask.LayerToName(gameObject.layer)}'"
        );

        glow = gameObject.AddComponent<GlowEffect>();
        glow.Initialize(outlineMaterial, targetRenderer);
        // TODO: make this dynamic, placeholder of 0 for now
        monkeys = new(0);
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        selectable = true;
    }

    protected virtual void UpdateBehavior()
    {

    }

    public virtual bool RemoveMonkey(MonkeyController m) => monkeys.Remove(m);
    public virtual void RemoveNextMonkey() => monkeys.Remove(NextMonkeyToRemove());

    // DON'T USE THIS, THIS IS ONLY TO BE USED IN MONKEYALLOC
    public virtual bool AddMonkey(MonkeyController monkey) => monkeys.Add(monkey);
    // AS A REMINDER, DO NOT USE THIS!

    public virtual MonkeyController NextMonkeyToRemove() => monkeys.MonkeyToDeallocate();

    public void EnableGlow() => glow.SetGlow(true);
    public void DisableGlow() => glow.SetGlow(false);

    public virtual int GetMonkeyCount() => monkeys.count;
    public virtual int GetMonkeyCapacity() => monkeys.capacity;
    public bool CanAllocate() => monkeys.CanAllocate();

    public BuildingType GetBuildingType() => building.type;
    public Building GetInternalBuilding() => building;

    public void SetVisible(bool visible)
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }
        // this is so we don't click on it by mistake
        foreach (Collider c in colliders)
        {
            c.enabled = visible;
        }
    }

    public void MoveMonkeysToPos(Vector3 newpos)
    {
        foreach(MonkeyController m in monkeys.monkeys)
        {
            m.transform.position = newpos;
        }
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