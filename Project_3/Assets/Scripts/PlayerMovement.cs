using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float RotateSpeed = 150f;
    public float JumpForce = 5f;
    public float GravityMultiplier = 2f;

    public float pMaxHealth = 100f;
    public float pCurrentHealth;

    //Invincible frames
    public float iFrameDuration;
    private float iFrameTimer;
    private bool isInvincible;

    private float _vInput;
    private float _hInput;
    private bool _isGrounded;
    private Rigidbody _rb;

    public Transform cameraTransform;

    public Slider slider;

    public Animator animator;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true; //prevent Rigidbody rotation to avoid camera shake
        pCurrentHealth = pMaxHealth;
        slider.maxValue = pMaxHealth;
        slider.value = pCurrentHealth;
    }

    void Update()
    {   
        if(iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }
        else if(isInvincible)
        {
            isInvincible = false;
        }

        //the vertical and horizontal input values
        _vInput = Input.GetAxis("Vertical") * MoveSpeed;
        _hInput = Input.GetAxis("Horizontal") * MoveSpeed;

        bool isMoving = _vInput != 0 || _hInput != 0;
        animator.SetBool("isWalking", isMoving);

        //jumping
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            _isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        //get camera direction
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        //combine the forward and right inputs with the camera's direction
        Vector3 direction = forward * _vInput + right * _hInput;

        //movement
        if (direction != Vector3.zero)
        {
            _rb.MovePosition(_rb.position + direction * Time.fixedDeltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, Time.fixedDeltaTime * RotateSpeed));
        }

        //manual gravity to avoid camera shake
        _rb.AddForce(Physics.gravity * GravityMultiplier, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision other) //detects when the player is on the ground
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }
    
    public void TakeDamage(float dmg)
    {
        if(!isInvincible)
        {
             pCurrentHealth -= dmg;

            slider.value = pCurrentHealth;

            if(pCurrentHealth <= 0)
            {
                PlayerDeath();
            }
        }
    }

    void PlayerDeath()
    {
        Destroy(this.gameObject);
    }
}
