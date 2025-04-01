using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbMovement : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;
    public float orbitSpeed = 50f;
    public float orbitRadius = 2f;
    public float verticalOffset = 1.0f;
    public float speedToHit = 10f;
    public float hoverOffset = 0.5f;
    public float returnSpeed = 10f;
    public Collider orbCollider;
    public List<GameObject> allEnemiesList = new List<GameObject>();

    private Vector3 targetPosition;
    private bool movingToTarget = false;
    private bool hasArrived = false;
    private bool returningToPlayer = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !returningToPlayer) //"F" key to recall the orb
        {
            StartReturningToPlayer();
        }

        if (movingToTarget)
        {
            MoveToTarget();
        }
        else if (returningToPlayer)
        {
            MoveBackToPlayer();
        }
        else if (!hasArrived)
        {
            OrbitPlayer();
        }
    }

    void OrbitPlayer() //orb orbits around the player
    {
        float angle = Time.time * orbitSpeed;
        float x = player.position.x + Mathf.Cos(angle) * orbitRadius;
        float z = player.position.z + Mathf.Sin(angle) * orbitRadius;
        float y = player.position.y + verticalOffset;

        transform.position = new Vector3(x, y, z);
    }

    void MoveToTarget() //moves orb to the location of the projectile
    {
        if (targetPosition != Vector3.zero)
        {
            Vector3 targetWithHover = targetPosition + Vector3.up * hoverOffset;
            transform.position = Vector3.MoveTowards(transform.position, targetWithHover, speedToHit * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetWithHover) < 0.1f)
            {
                movingToTarget = false;
                hasArrived = true;
            }
        }
    }

    void MoveBackToPlayer() //moves orb back to player smoothly
    {
        Vector3 playerPositionWithOffset = player.position + Vector3.up * verticalOffset;
        transform.position = Vector3.MoveTowards(transform.position, playerPositionWithOffset, returnSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, playerPositionWithOffset) < 0.1f)
        {
            returningToPlayer = false;
            hasArrived = false; //resume orbiting
        }
    }

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        movingToTarget = true;
        hasArrived = false;
    }

    public void StartReturningToPlayer() //begins the return process
    {
        returningToPlayer = true;
        movingToTarget = false;
        hasArrived = false;
    }

    void OnTriggerEnter(Collider other) //destroys enemies upon collision
    {
        if (other.CompareTag("Enemy"))
        {
            allEnemiesList.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }

    public bool HasArrived()
    {
        return hasArrived;
    }

    public void SetArrivedState(bool state)
    {
        hasArrived = state;
    }
}
