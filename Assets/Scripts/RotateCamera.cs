using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    float rotationStep = 90f;  // rotate in 90° increments
    Transform camTransform;
    Vector3 pivot;
    [SerializeField] private Transform lightingTransform;

    void Start()
    {
        camTransform = Camera.main.transform;
        pivot = BuildingGrid.instance.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) RotateCameraLeft();
        else if (Input.GetKeyDown(KeyCode.RightArrow)) RotateCameraRight();
    }

    void RotateCameraLeft()
    {
        RotateAroundPivot(camTransform, -rotationStep);
        RotateAroundPivot(lightingTransform, -rotationStep);
    }
    
    void RotateCameraRight()
    {
        RotateAroundPivot(camTransform, rotationStep);
        RotateAroundPivot(lightingTransform, rotationStep);
    }

    void RotateAroundPivot(Transform target, float angle)
    {
        if (target == null) return;

        target.RotateAround(pivot, Vector3.up, angle);
        target.LookAt(pivot);
    }
}
