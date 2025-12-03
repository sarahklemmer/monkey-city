using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BuildingBase : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Renderer targetRenderer;
    private GlowEffect glow;
    public BuildingMonkeys monkeys {get; protected set; }
    protected VisualMonkeys monkeyVisualizer;
    protected Renderer[] renderers;
    protected Collider[] colliders;
    private bool SharedAwakeBehaviorCalled = false;
    public bool selectable { get; private set; }
    public bool canContainMonkeys { get; protected set; } = true;
    public bool canNeverBeUpgraded { get; protected set; } = true;
    public int bananasPerDay { get; protected set; } = 0;
    // functions to be overrode
    public abstract void OnDayCycle();
    public abstract void OnDestroy();
    public BuildingType type {get; protected set;}
    public BuildingHealth health;

    public int grid_x { get; private set; } = 0;
    public int grid_y { get; private set; } = 0;

    void Update()
    {
        Assert.IsTrue(SharedAwakeBehaviorCalled, "forgot to call sharedawakebehavior in class that extends buildingbase");
        UpdateBehavior();
    }

    // basically RAII for the BuildingManager, super handy
    protected virtual void OnEnable()
    {
        BuildingManager.instance.AddBuilding(this);
        if(health == null) health = GetComponent<BuildingHealth>();
        health.SetBuilding(this);
    }

    protected virtual void OnDisable()
    {
        BuildingManager.instance.RemoveBuilding(this);
        BuildingGrid.instance.RemoveBuilding(this);
    }

    public virtual void OnTakeDamage()
    {
        
    }

    protected virtual void SharedAwakeBehavior()
    {
        SharedAwakeBehaviorCalled = true;
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
        monkeyVisualizer = GetComponent<VisualMonkeys>();
        selectable = true;
    }

    protected virtual void UpdateBehavior()
    {

    }

    public virtual bool RemoveMonkey(MonkeyController m) {
        if(!monkeys.Remove(m)) return false;
        if(monkeyVisualizer != null) monkeyVisualizer.SetMonkeyCount(monkeys.Count());
        return true;
    }

    public virtual void RemoveNextMonkey() => monkeys.Remove(NextMonkeyToRemove());

    // DON'T USE THIS, THIS IS ONLY TO BE USED IN MONKEYALLOC
    public virtual bool AddMonkey(MonkeyController monkey) {
        if(!monkeys.Add(monkey)) return false;
        if(monkeyVisualizer != null) monkeyVisualizer.SetMonkeyCount(monkeys.Count());
        return true;
    }
    // AS A REMINDER, DO NOT USE THIS!

    public virtual MonkeyController NextMonkeyToRemove() => monkeys.MonkeyToDeallocate();

    public void EnableGlow() => glow.SetGlow(true);
    public void DisableGlow() => glow.SetGlow(false);

    public virtual int GetMonkeyCount() => monkeys.count;
    public virtual int GetMonkeyCapacity() => monkeys.capacity;
    public virtual void OnSelectOrView() {}
    public virtual void OnDeslectOrStopViewing() {}
    public bool CanAllocate() => monkeys.CanAllocate();

    public BuildingType GetBuildingType() => type;

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
        if(monkeyVisualizer != null) monkeyVisualizer.SetMonkeyCount(monkeys.Count());
    }

    public void MoveMonkeysToPos(Vector3 newpos)
    {
        foreach(MonkeyController m in monkeys.monkeys)
        {
            m.transform.position = newpos;
        }
    } 

    public void SetGridCoords(int grid_x, int grid_y)
    {
        this.grid_x = grid_x;
        this.grid_y = grid_y;
    }

    public abstract string GetDescription();
    public virtual bool CanUpgrade() => false;
    public virtual bool AttemptUpgrade() => false;
    public virtual int GetUpgradeCost() => 0;
    public virtual string GetUpgradeText() => 
        "This building cannot be upgraded any further";
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