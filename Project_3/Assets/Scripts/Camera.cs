using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Vector3 CamOffset = new Vector3(0f, 1.2f, -2.6f);
    public float mouseSensitivity = 100f;

    private Transform _target;
    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        _target = GameObject.Find("Player").transform;
        Cursor.lockState = CursorLockMode.Locked; //locks the cursor
    }

    void LateUpdate()
    {
        //get mouse input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -45f, 45f); //limit vertical camera rotation

        //rotate the camera based on mouse input
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = _target.position + rotation * CamOffset;
        transform.LookAt(_target);
    }
}
