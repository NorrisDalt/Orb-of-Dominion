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
    public Collider orbCollider;
    public List<GameObject> allEnemiesList = new List<GameObject>();

    private Vector3 targetPosition;
    private bool movingToTarget = false;
    private bool hasArrived = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) //"F" key to recall the orb
        {
            RecallOrb();
        }

        if (movingToTarget)
        {
            MoveToTarget();
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

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        movingToTarget = true;
        hasArrived = false;
    }

    public void RecallOrb() //returns the orb to its orbiting state
    {
        movingToTarget = false;
        hasArrived = false;
        targetPosition = Vector3.zero;
    }

    void OnTriggerEnter(Collider other) //destroys enemies upon collision
    {
        if (other.CompareTag("Enemy"))
        {
            allEnemiesList.Remove(other.gameObject);
            Destroy(other.gameObject);
        }
    }
}
