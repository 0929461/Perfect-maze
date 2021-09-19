using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float _speed=10.0f;
    
    void Update()
    {
        ControlCamera();
    }

    private void ControlCamera()
    {
        if(Input.GetAxis("Mouse X") > 0)
        {
            transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * _speed, 0.0f, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _speed);

        } 
        else if (Input.GetAxis("Mouse X") < 0)
        {
            transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * _speed, 0.0f, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * _speed);
        }
    }
}
