using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    public Vector3 CamOffset = new Vector3(0.6f, 1.5f, -3.5f); // over-the-shoulder feel
    public float mouseSensitivity = 1f;
    public float controllerSensitivity = 1f;

    private Transform _target;
    private float pitch = 0f;
    private float yaw = 0f;

    private Vector2 lookInput;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += _ => lookInput = Vector2.zero;
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        _target = GameObject.Find("Player 1").transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        //get mouse input
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -45f, 45f); //limit vertical camera rotation
        // Check whether mouse or controller is being used
        float sensitivity = IsUsingMouse() ? mouseSensitivity : controllerSensitivity;

        // Apply look input with the appropriate sensitivity
        yaw += lookInput.x * sensitivity;
        pitch -= lookInput.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -45f, 45f);

        //rotate the camera based on mouse input
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.position = _target.position + rotation * CamOffset;
        transform.LookAt(_target);
        transform.LookAt(_target.position + Vector3.up * 1.2f); // eye level
    }

    private bool IsUsingMouse()
    {
        // Check if mouse input is detected by the system
        return Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;
    }
}