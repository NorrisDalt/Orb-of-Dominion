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

    public float damage = 15;

    public float cooldownTime = 3f;
    private float cooldownTimer = 0f;

    private float maxHealth = 100f;
    private float currentHealth = 0;

    //Invincible frames
    public float iFrameDuration;
    private float iFrameTimer;
    private bool isInvincible;



    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();

        currentHealth = maxHealth;
        
    }

    
    void Update()
    {
        agent.SetDestination(playerPos.position);
        TypeOfAgent();

        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if(iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }
        //If the Invinciblity timer reaches 0 , set invinsiblity bool to false
        else if(isInvincible)
        {
            isInvincible = false;
        }

    }

    void TypeOfAgent()
    {
        if(agent.agentTypeID == NavMesh.GetSettingsByIndex(2).agentTypeID)
        {
            if(agent.velocity.magnitude == 0 && cooldownTimer <= 0)
            {
                GameObject clone;
                clone = Instantiate(bullet, fireLoc.position, transform.rotation);

                Vector3 directionToPlayer = (playerPos.position - fireLoc.position).normalized;

                clone.GetComponent<Rigidbody>().velocity = directionToPlayer * 10f;
                Destroy(clone,2.5f);

                cooldownTimer = cooldownTime;
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        if(!isInvincible)
        {
            currentHealth -= dmg; 

            iFrameTimer = iFrameDuration;
            isInvincible = true;

            if(currentHealth <= 0)
            {
                Death();
            }
        }
        
        if(currentHealth <= 0)
        {
            Death();
        }
    }

    void Death()
    {
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player"))
        {
            PlayerMovement player = col.GetComponent<PlayerMovement>();
            player.TakeDamage(damage);
        }
    }

}
