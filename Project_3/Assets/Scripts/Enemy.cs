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

    public float cooldownTime = 3f;
    private float cooldownTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        
    }

    
    void Update()
    {
        agent.SetDestination(playerPos.position);
        TypeOfAgent();

        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
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

}
