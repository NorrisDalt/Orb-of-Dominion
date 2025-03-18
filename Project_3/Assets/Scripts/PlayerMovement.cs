using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float RotateSpeed = 150f;
    private float _vInput; //vertical input
    private float _hInput; //horizontal input

    public Transform cameraTransform; //the camera's transform

    void Update()
    {
        _vInput = Input.GetAxis("Vertical") * MoveSpeed;
        _hInput = Input.GetAxis("Horizontal") * MoveSpeed;

        //get the camera's forward and right vectors
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        //flatten the vectors
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        //move based on camera's orientation
        Vector3 direction = forward * _vInput + right * _hInput;
        if (direction != Vector3.zero)
        {
            transform.Translate(direction * Time.deltaTime, Space.World);

            //rotate player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * RotateSpeed);
        }
    }
}
