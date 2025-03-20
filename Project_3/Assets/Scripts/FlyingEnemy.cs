using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public GameObject flyingType;
    public UnityEngine.AI.NavMeshAgent agent;
    private Transform playerPos;

    private float targetY = 3.5f; // Target Y position
    private float startY = 3.0f; // Starting Y position
    private float timer = 0f;    // Timer for interpolation
    public float duration = 2f; // Duration of the movement cycle


    void Update()
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        if (flyingType != null)
        {
            // Increment the timer
            timer += Time.deltaTime;

            // Calculate the interpolation factor (0 to 1)
            float t = Mathf.PingPong(timer / duration, 1f);

            // Interpolate the Y position between startY and targetY
            Vector3 newPosition = flyingType.transform.position;
            newPosition.y = Mathf.Lerp(startY, targetY, t);

            // Update the position
            flyingType.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("FlyingType is not assigned!");
        }

        agent.SetDestination(playerPos.position);
    }
}