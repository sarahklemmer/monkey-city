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
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) )
        {
            currentLeftSpeed = Mathf.Min(currentLeftSpeed + holdAcceleration * Time.deltaTime, maxRotationSpeed);
            RotateCameraLeft(currentLeftSpeed * Time.deltaTime);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            currentLeftSpeed = 0f;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            currentRightSpeed = Mathf.Min(currentRightSpeed + holdAcceleration * Time.deltaTime, maxRotationSpeed);
            RotateCameraRight(currentRightSpeed * Time.deltaTime);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            currentRightSpeed = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) )
        {
            currentLeftSpeed = baseRotationSpeed;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
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