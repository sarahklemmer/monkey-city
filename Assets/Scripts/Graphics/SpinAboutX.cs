using UnityEngine;

public class RotateAroundX : MonoBehaviour
{
    [SerializeField] private float speed = 90f; // degrees per second

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime, 0f, 0f, Space.Self);
    }
}