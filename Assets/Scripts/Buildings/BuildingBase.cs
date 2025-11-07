using UnityEngine;

public abstract class BuildingBase : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Renderer targetRenderer;
    private GlowEffect glow;

    protected Building building;
    protected BuildingMonkeys monkeys;

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
    }

    protected virtual void UpdateBehavior()
    {

    }
    
    public virtual void Remove()
    {
        if (monkeys.count <= 0) return;
        --monkeys.count;
        //TODO: other removal stuff
    }

    void OnMouseDown()
    {
        BuildingSelector.instance.Select(this);
    }

    public void EnableGlow()
    {
        glow.SetGlow(true);
    }

    public void DisableGlow()
    {
        glow.SetGlow(false);
    }
}
