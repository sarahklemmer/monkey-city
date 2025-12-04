using UnityEngine;
using UnityEngine.Assertions;

public class GameManagementHeirarchy : MonoBehaviour
{
    [SerializeField] GameObject buildingList;
    [SerializeField] GameObject infoPopup;

    public static GameManagementHeirarchy instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("duplicate GameManagementHeirarchy on " + gameObject.name + " destroying.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        Assert.IsNotNull(buildingList, "buildingList not assigned!");
        Assert.IsNotNull(infoPopup, "infoPopup not assigned!");

        Assert.AreEqual(transform, buildingList.transform.parent, "buildingList is not a child of this GameObject!");
        Assert.AreEqual(transform, infoPopup.transform.parent, "infoPopup is not a child of this GameObject!");

        int count = transform.childCount;
        Assert.IsTrue(count >= 2, "Parent must have at least two children.");

        Transform secondLast = transform.GetChild(count - 2);
        Transform last       = transform.GetChild(count - 1);

        Assert.AreEqual(secondLast, buildingList.transform, "buildingList must be second-to-last child at start.");
        Assert.AreEqual(last, infoPopup.transform, "infoPopup must be last child at start.");

        // just 2 b safe
        MakeBuildingListLastChild();
    }

    public void MakeBuildingListLastChild()
    {
        Assert.AreEqual(transform, buildingList.transform.parent);
        Assert.AreEqual(transform, infoPopup.transform.parent);

        infoPopup.transform.SetSiblingIndex(transform.childCount - 2);
        buildingList.transform.SetSiblingIndex(transform.childCount - 1);
    }

    public void MakeInfoLastChild()
    {
        Assert.AreEqual(transform, buildingList.transform.parent);
        Assert.AreEqual(transform, infoPopup.transform.parent);

        buildingList.transform.SetSiblingIndex(transform.childCount - 2);
        infoPopup.transform.SetSiblingIndex(transform.childCount - 1);
    }
}