using UnityEngine;

public class AllMonkeyInfo : MonoBehaviour
{
    public static AllMonkeyInfo instance;

    [SerializeField] private float baseMonkeySpeed = 5;

    //[SerializeField] private float costMultiplier = 1.0f;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate AllMonkeyInfo on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public float GetMonkeySpeed()
    {
        return baseMonkeySpeed;
    }

    public void IncreaseMonkeySpeed(float value)
    {
        baseMonkeySpeed += value;
    }
}

