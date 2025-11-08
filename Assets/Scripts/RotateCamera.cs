using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    float rotationStep = 90f;  // rotate in 90° increments
    Transform camTransform;
    Vector3 pivot;

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
        RotateAroundPivot(-rotationStep);
    }
    
    void RotateCameraRight()
    {
        RotateAroundPivot(rotationStep);
    }

    void RotateAroundPivot(float angle)
    {
        camTransform.RotateAround(pivot, Vector3.up, angle);
        camTransform.LookAt(pivot);
    }
}
