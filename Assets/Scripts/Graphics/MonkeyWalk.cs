using UnityEngine;

public class MonkeyWalk : MonoBehaviour
{
    [SerializeField] float radius = 0.08f;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float offsetDegrees = 0f;

    private float direction = 1;
    private float theta = 0;
    private float offset;
    
    void Start()
    {
        offset = Mathf.PI * offsetDegrees;
        offset /= 180f;
    }

    void Update()
    {   
        theta += direction * walkSpeed * Time.deltaTime;
        transform.localPosition = new Vector3(Mathf.Cos(theta + offset) * radius, Mathf.Sin(theta + offset) * radius, 0);

        Vector3 tangent = direction * new Vector3( -Mathf.Sin(theta + offset), Mathf.Cos(theta + offset), 0f);

        float angleDeg = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(angleDeg, 0f, 0f);
    }
}
