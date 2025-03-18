using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : MonoBehaviour
{
    public GameObject flyingType;
    private NavMeshAgent agent;

    public Transform playerPos; 

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = flyingType.transform.position;

       // newPosition.y = Vector3.MoveTowards()

        flyingType.transform.position = newPosition;

        agent.SetDestination(playerPos.position);
    }
}
