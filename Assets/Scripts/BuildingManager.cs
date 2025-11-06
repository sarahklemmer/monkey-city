using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager instance;
    List<BuildingBase> buildings;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate BuildingManager on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;

        buildings = new List<BuildingBase>();
    }

    public void RemoveBuilding(BuildingBase b) => buildings.Remove(b);


    public void AddBuilding(BuildingBase b) => buildings.Add(b);

    public void PassDay()
    {
        foreach (BuildingBase b in buildings)
        {
            b.OnDayCycle();
        }
    }
}
