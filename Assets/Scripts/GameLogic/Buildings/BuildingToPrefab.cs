using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class BuildingToPrefab : MonoBehaviour
{
    [System.Serializable]
    class BuildingValues
    {
        public GameObject prefab;
        public Sprite icon;
    }

    [System.Serializable]
    class BuildingTypeToValues
    {
        public BuildingType key;
        public BuildingValues values;
    }

    [SerializeField] List<BuildingTypeToValues> mappings;
    private static Dictionary<BuildingType, BuildingValues> map = new();

    void Awake()
    {
        Assert.AreEqual(
            mappings.Count,
            System.Enum.GetValues(typeof(BuildingType)).Length,
            "BuildingToPrefab doesn't align with number of building types, either extras are assigned in the editor or you forgot to add a mapping when you made a new type"
        );

        foreach (BuildingTypeToValues m in mappings)
        {
            map[m.key] = m.values;
        }
    }
    
    public static GameObject GetPrefab(BuildingType type)
    {
        Assert.IsTrue(map.ContainsKey(type), $"Prefab mapping missing for {type}");
        return map[type].prefab;
    }

    public static Sprite GetIcon(BuildingType type)
    {
        Assert.IsTrue(map.ContainsKey(type), $"Prefab mapping missing for {type}");
        return map[type].icon;
    }
}
