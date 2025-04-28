using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float RotateSpeed = 150f;
    public float JumpForce = 5f;
    public float GravityMultiplier = 2f;

    public float pMaxHealth = 100f;
    public float pCurrentHealth;

    // Invincible frames
    public float iFrameDuration;
    private float iFrameTimer;
    private bool isInvincible;

    private Vector2 moveInput;
    private bool jumpPressed = false;

    private bool _isGrounded;
    private Rigidbody _rb;

    public Transform cameraTransform;

    public Slider slider;

    public Animator animator;

    private PlayerControls controls;

    // --- Walking and Jumping Sound Variables ---
    public AudioSource walkAudioSource;
    public AudioClip walkClip;
    public AudioClip jumpClip;
    // --------------------------------------------

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => jumpPressed = true;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        pCurrentHealth = pMaxHealth;

        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform is not assigned!");
        }

        _rb.freezeRotation = true; // Prevent Rigidbody rotation to avoid camera shake

        if (walkAudioSource != null)
        {
            walkAudioSource.clip = walkClip;
            walkAudioSource.loop = true;
            walkAudioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        GameObject sliderObj = GameObject.Find("Health"); // Temporary
        if (sliderObj != null)
        {
            slider = sliderObj.GetComponent<Slider>();
            if (slider != null)
            {
                slider.maxValue = pMaxHealth;
                slider.value = pCurrentHealth;
            }
        }
        else
        {
            Debug.LogWarning("Health slider not found!");
        }

        if (iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }

        bool isMoving = moveInput != Vector2.zero;
        animator.SetBool("isWalking", isMoving);

        // --- Walking Sound Logic ---
        if (isMoving && _isGrounded)
        {
            if (walkAudioSource != null && !walkAudioSource.isPlaying)
            {
                walkAudioSource.Play();
            }
        }
        else
        {
            if (walkAudioSource != null && walkAudioSource.isPlaying)
            {
                walkAudioSource.Pause();
            }
        }
        // ----------------------------

        if (jumpPressed && _isGrounded)
        {
            _rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);

            // --- Play Jump Sound Instantly ---
            if (jumpClip != null)
            {
                AudioSource.PlayClipAtPoint(jumpClip, transform.position);
            }
            // ----------------------------------

            _isGrounded = false;
            jumpPressed = false;
        }
    }

    void FixedUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 direction = forward * moveInput.y + right * moveInput.x;

        if (direction != Vector3.zero)
        {
            _rb.MovePosition(_rb.position + direction * MoveSpeed * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * RotateSpeed));
        }

        _rb.AddForce(Physics.gravity * GravityMultiplier, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            pCurrentHealth -= dmg;
            slider.value = pCurrentHealth;

            if (pCurrentHealth <= 0)
            {
                PlayerDeath();
            }
        }
    }

    void PlayerDeath()
    {
        Destroy(this.gameObject);
        SceneManager.LoadScene(7);
    }
}
