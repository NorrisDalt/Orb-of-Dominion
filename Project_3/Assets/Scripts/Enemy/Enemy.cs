using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform playerPos;
    private NavMeshAgent agent;
    public Rigidbody rb;
    public GameObject bullet;
    public Transform fireLoc;
    private AudioSource audioSource;

    public AudioClip audioClip;
    public AudioClip damageSound;
    public GameObject damageEffect;
    public GameObject damageTextPrefab;

    public float damage = 15;
    public float cooldownTime = 3f;
    private float cooldownTimer = 0f;

    private float maxHealth = 500f;
    public float currentHealth = 0;

    // Invincible frames
    public float iFrameDuration;
    private float iFrameTimer;
    private bool isInvincible;
    private bool isPlayingSound;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        
        // Safe audio source initialization
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource was missing - created one automatically", this);
        }

        currentHealth = maxHealth;
    }

    void Update()
    {
        agent.SetDestination(playerPos.position);
        TypeOfAgent();

        // Safe walking sound playback with null checks
        if (agent.velocity.magnitude > 0.1f && !isPlayingSound)
        {
            if (audioSource != null && audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
                isPlayingSound = true;
            }
            else
            {
                Debug.LogWarning("Missing audio components for walking sound", this);
            }
        }
        else if (agent.velocity.magnitude <= 0.1f)
        {
            isPlayingSound = false;
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }
        else if (isInvincible)
        {
            isInvincible = false;
        }
    }

    void TypeOfAgent()
    {
        if (agent.agentTypeID == NavMesh.GetSettingsByIndex(2).agentTypeID)
        {
            if (agent.velocity.magnitude == 0 && cooldownTimer <= 0)
            {
                GameObject clone = Instantiate(bullet, fireLoc.position, transform.rotation);
                Vector3 directionToPlayer = (playerPos.position - fireLoc.position).normalized;
                clone.GetComponent<Rigidbody>().velocity = directionToPlayer * 10f;
                Destroy(clone, 2.5f);
                cooldownTimer = cooldownTime;
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            currentHealth -= dmg;

            // Safe damage sound playback
            if (audioSource != null && damageSound != null)
            {
                audioSource.PlayOneShot(damageSound);
            }

            // Safe particle effect instantiation
            if (damageEffect != null)
            {
                Instantiate(damageEffect, transform.position, Quaternion.identity);
            }

            iFrameTimer = iFrameDuration;
            isInvincible = true;

            if (currentHealth <= 0)
            {
                Death();
            }
        }

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerMovement player = col.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}