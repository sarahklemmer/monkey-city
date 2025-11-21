using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class BuildingToPrefab : MonoBehaviour
{
    [System.Serializable]
    class BuildingTypeToPrefab
    {
        public BuildingType key;
        public GameObject value;
    }

    [SerializeField] List<BuildingTypeToPrefab> mappings;
    private static  Dictionary<BuildingType, GameObject> map = new();

    void Awake()
    {
        Assert.AreEqual(
            mappings.Count,
            System.Enum.GetValues(typeof(BuildingType)).Length,
            "BuildingToPrefab doesn't align with number of building types, either extras are assigned in the editor or you forgot to add a mapping when you made a new type"
        );

        foreach (BuildingTypeToPrefab m in mappings)
        {
            map[m.key] = m.value;
        }
    }
    
    public static GameObject GetPrefab(BuildingType type)
    {
        Assert.IsTrue(map.ContainsKey(type), $"Prefab mapping missing for {type}");
        return map[type];
    }
}
