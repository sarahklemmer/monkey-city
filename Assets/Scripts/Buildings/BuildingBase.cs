using UnityEngine;
using UnityEngine.Assertions;

public abstract class BuildingBase : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Renderer targetRenderer;
    [Header("Occupancy Models")]
    [SerializeField] protected GameObject[] occupancyModels;
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
    }

    protected virtual void SharedAwakeBehavior()
    {
        Assert.IsTrue(
            gameObject.layer == LayerMask.NameToLayer("Building"),
            $"{gameObject.name} MUST be on the 'Building' layer, but is currently on '{LayerMask.LayerToName(gameObject.layer)}'"
        );

        glow = gameObject.AddComponent<GlowEffect>();
        glow.Initialize(outlineMaterial, targetRenderer);
        // TODO: make this dynamic, placeholder of 2 for now
        monkeys = new(2);
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        selectable = true;

        UpdateOccupancyModel();
    }

    protected virtual void UpdateBehavior()
    {

    }

    public virtual bool RemoveMonkey(MonkeyController m)
    {
        bool removed = monkeys.Remove(m);
        if (removed) UpdateOccupancyModel();
        return removed;
    }

    public virtual void RemoveNextMonkey()
    {
        monkeys.Remove(NextMonkeyToRemove());
    }

    // DON'T USE THIS, THIS IS ONLY TO BE USED IN MONKEYALLOC
    public virtual bool AddMonkey(MonkeyController monkey)
    {
        bool added = monkeys.Add(monkey);
        if (added) UpdateOccupancyModel();
        return added;
    }
    
    public virtual MonkeyController NextMonkeyToRemove()
    {
        return monkeys.MonkeyToDeallocate();
    }

    protected virtual void UpdateOccupancyModel()
    {
        Debug.Log("Updating occupancy model for " + gameObject.name);
        if (occupancyModels == null || occupancyModels.Length == 0 || monkeys == null)
            Debug.Log("No occupancy models or monkeys data found for " + gameObject.name);
            return;
        float fillRatio = monkeys.CountOverCapacity();
        int modelIndex = Mathf.Clamp(
            Mathf.FloorToInt(fillRatio * occupancyModels.Length),
            0,
            occupancyModels.Length - 1
        );
        
        for (int i = 0; i < occupancyModels.Length; i++)
        {
            if (occupancyModels[i] != null)
            {
                occupancyModels[i].SetActive(i == modelIndex);
            }
        }
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

    public bool CanAllocate()
    {
        return monkeys?.CanAllocate() ?? false;
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