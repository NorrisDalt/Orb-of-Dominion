using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbMovement : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;
    public float orbitSpeed = 50f;
    public float orbitRadius = 2f;
    public float verticalOffset = 1.0f; //moves the orb up slightly
    public float speedToHit = 10f; //speed the orb moves to its target
    public float hoverOffset = 0.5f; //vertical offset so the orb doesn't stick in the ground
    public float returnSpeed = 10f; //return to player speed

    public Collider orbCollider;

    private Vector3 targetPosition; //the position the orb is moving to
    private Transform enemyTarget = null; //for tracking the enemy
    private bool movingToTarget = false;
    private bool hasArrived = false;
    private bool returningToPlayer = false;

    public List<GameObject> allEnemiesList = new List<GameObject>();

    void Update()
    {
        //"F" key to recall the orb back to the player
        if (Input.GetKeyDown(KeyCode.F) && !returningToPlayer)
        {
            StartReturningToPlayer();
        }

        //moves the orb based on its current state
        if (movingToTarget)
        {
            MoveToTarget(); //moving state
        }
        else if (enemyTarget != null)
        {
            FollowEnemy(); //tracking enemy state
        }
        else if (returningToPlayer)
        {
            MoveBackToPlayer(); //return state
        }
        else if (!hasArrived)
        {
            OrbitPlayer(); //orbiting state
        }
    }

    void OrbitPlayer() //orbits orb around the player
    {
        float angle = Time.time * orbitSpeed;
        float x = player.position.x + Mathf.Cos(angle) * orbitRadius;
        float z = player.position.z + Mathf.Sin(angle) * orbitRadius;
        float y = player.position.y + verticalOffset;

        transform.position = new Vector3(x, y, z);
    }

    //moves the orb to its current target
    void MoveToTarget()
    {
        if (targetPosition != Vector3.zero)
        {
            Vector3 targetWithHover = targetPosition + Vector3.up * hoverOffset; //adds vertical offset so the orb doesn't go into the ground
            transform.position = Vector3.MoveTowards(transform.position, targetWithHover, speedToHit * Time.deltaTime); //moves the orb to the target

            //checks if orb has arrived
            if (Vector3.Distance(transform.position, targetWithHover) < 0.1f)
            {
                movingToTarget = false; //stops the orb once it has arrived
                hasArrived = true; //set the orbs state to "arrived"
            }
        }
    }

    void FollowEnemy() //follows current enemy target
    {
        if (enemyTarget == null) //if enemy is already destroyed stop following
        {
            return;
        }

        //move the orb towards the enemy
        Vector3 enemyPositionWithHover = enemyTarget.position;
        transform.position = Vector3.MoveTowards(transform.position, enemyPositionWithHover, speedToHit * Time.deltaTime);
    }

    void MoveBackToPlayer() //moves the orb back to the player
    {
        Vector3 playerPositionWithOffset = player.position + Vector3.up * verticalOffset; //find the players position with a vertical offset to avoid it dragging on the ground

        transform.position = Vector3.MoveTowards(transform.position, playerPositionWithOffset, returnSpeed * Time.deltaTime); //move orb to player

        if (Vector3.Distance(transform.position, playerPositionWithOffset) < 0.1f) //orb stops returning once it reaches the player
        {
            returningToPlayer = false;
            hasArrived = false;
        }
    }

    //sets the target for the orb (Ground or Enemy)
    public void SetTargetPosition(Vector3 position, Transform enemy = null)
    {
        if (enemy != null) //enemy
        {
            enemyTarget = enemy;
            movingToTarget = false;
            hasArrived = false;
        }
        else //ground
        {
            targetPosition = position;
            movingToTarget = true;
            hasArrived = false;
        }
    }

    //prepares all of the orbs states for returning to the player
    void StartReturningToPlayer()
    {
        returningToPlayer = true;
        movingToTarget = false;
        hasArrived = false;
        enemyTarget = null;
    }

    //detects the collision with enemies and the ground
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); //destroys enemy

            if (other.transform == enemyTarget)
            {
                enemyTarget = null;
                hasArrived = false;
                StartReturningToPlayer(); //orb returns to player after enemy is destroyed
            }
        }
        else if (other.CompareTag("Ground")) //if the orb hits ground, stay in position until "F" is pressed
        {
            targetPosition = transform.position;
            movingToTarget = false;
        }
    }

    public bool HasArrived() //return whether the orb has arrived at its target
    {
        return hasArrived;
    }

    public void SetArrivedState(bool state) //sets state of orb
    {
        hasArrived = state;
    }
}
