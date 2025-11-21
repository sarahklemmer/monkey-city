using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float baseRotationSpeed = 30f;
    [SerializeField] private float holdAcceleration = 60f;
    [SerializeField] private float maxRotationSpeed = 180f; 
    
    Transform camTransform;
    Vector3 pivot;
    [SerializeField] private Transform lightingTransform;

    private float currentLeftSpeed = 0f;
    private float currentRightSpeed = 0f;

    void Start()
    {
        camTransform = Camera.main.transform;
        pivot = BuildingGrid.instance.transform.position;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentLeftSpeed = Mathf.Min(currentLeftSpeed + holdAcceleration * Time.deltaTime, maxRotationSpeed);
            RotateCameraLeft(currentLeftSpeed * Time.deltaTime);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            currentLeftSpeed = 0f;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentRightSpeed = Mathf.Min(currentRightSpeed + holdAcceleration * Time.deltaTime, maxRotationSpeed);
            RotateCameraRight(currentRightSpeed * Time.deltaTime);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            currentRightSpeed = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentLeftSpeed = baseRotationSpeed;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentRightSpeed = baseRotationSpeed;
        }
    }

    void RotateCameraLeft(float angle)
    {
        RotateAroundPivot(camTransform, -angle);
        RotateAroundPivot(lightingTransform, -angle);
    }
    
    void RotateCameraRight(float angle)
    {
        RotateAroundPivot(camTransform, angle);
        RotateAroundPivot(lightingTransform, angle);
    }

    void RotateAroundPivot(Transform target, float angle)
    {
        if (target == null) return;

        target.RotateAround(pivot, Vector3.up, angle);
        target.LookAt(pivot);
    }
}