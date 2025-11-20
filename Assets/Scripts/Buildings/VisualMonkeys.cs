using UnityEngine;
using UnityEngine.Assertions;

public class BuildingVisualMonkeys : MonoBehaviour
{
    [SerializeField] private GameObject model0;
    [SerializeField] private GameObject model1;
    [SerializeField] private GameObject model2;

    private GameObject currentModel;

    void Start() { SetMonkeyCount(0); }

    public void SetMonkeyCount(int count)
    {  
        Assert.IsTrue(0 <= count && count <= 2, "setting monkey count too high in visualmonkeys!");

        GameObject target = count == 0 ? model0 : (count == 1 ? model1 : model2);

        if (currentModel == target) return;
        if (currentModel != null)
            Destroy(currentModel);

        if (target != null)
        {
            currentModel = Instantiate(target, transform);
            currentModel.transform.localPosition = Vector3.zero;
            currentModel.transform.localRotation = Quaternion.identity;
        }
    }
}
