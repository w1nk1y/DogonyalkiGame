using UnityEngine;

public class RotateObject : MonoBehaviour
{
    void Update()
    {
        float currentAngle = transform.eulerAngles.y;
        float newAngle = -90 + currentAngle;    
        
        transform.eulerAngles = new Vector3(newAngle, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}