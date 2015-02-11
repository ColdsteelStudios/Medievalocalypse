// ---------------------------------------------------------------------------
// HarleyMouseOrbit.cs
// 
// C# rewrite of unity's MouseOrbit script
// Has support for controllers to use the right thumbstick to move the camera
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class HarleyMouseOrbit : MonoBehaviour 
{
    public Transform Target;
    public float Distance = 10.0f;
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    public float yMinLimit = -20.0f;
    public float yMaxLimit = 80.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.x;
        y = angles.y;

        //Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
        Mouse();
        Controller();   
    }

    private void Mouse()
    {
        if (Target)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -Distance) + Target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private void Controller()
    {
        if (Target)
        {
            x += Input.GetAxis("Right Joystick X") * xSpeed * 0.02f;
            y += Input.GetAxis("Right Joystick Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -Distance) + Target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f)
            angle += 360.0f;
        if (angle > 360.0f)
            angle -= 360.0f;
        return Mathf.Clamp(angle, min, max);
    }
}