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
    public float orbDamage = 15f;
    public float slashDistance = 3f;
    public float slashSpeed = 20f;

    public Collider orbCollider;

    private Vector3 targetPosition; //the position the orb is moving to
    public Transform enemyTarget = null; //for tracking the enemy
    public bool movingToTarget = false;
    private bool hasArrived = false;
    private bool returningToPlayer = false;
    private bool isSlashing = false;
    private bool returningToOrbit = false;
    private Vector3 slashTargetPosition;

    public List<GameObject> allEnemiesList = new List<GameObject>();

    void Update()
    {
        // Recall orb with F
        if (Input.GetKeyDown(KeyCode.F) && !returningToPlayer)
        {
            StartReturningToPlayer();
        }

        // Only slash if orb is orbiting the player
        if (Input.GetKeyDown(KeyCode.E) && !movingToTarget && !returningToPlayer && enemyTarget == null && !hasArrived && !isSlashing)
        {
            StartCoroutine(TripleSlash());
        }

        if (isSlashing)
        {
            return; // don't update other movement while slashing
        }

        if (returningToOrbit)
        {
            ReturnToOrbit();
            return;
        }

        if (movingToTarget)
        {
            MoveToTarget();
        }
        else if (enemyTarget != null)
        {
            FollowEnemy();
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

    public void FollowEnemy() //follows current enemy target
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
            Enemy enemy = other.GetComponent<Enemy>(); // detect enemy script on enemy
            enemy.TakeDamage(orbDamage); //destroys enemy

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
        else if(other.CompareTag("FlyingEnemy"))
        {
            FlyingEnemy flyingEnemy = other.GetComponent<FlyingEnemy>(); // detect enemy script on enemy
            flyingEnemy.TakeDamage(orbDamage); //destroys enemy

            if (other.transform == enemyTarget)
            {
                enemyTarget = null;
                hasArrived = false;
                StartReturningToPlayer(); //orb returns to player after enemy is destroyed
            }
        }
    }

    private IEnumerator TripleSlash()
    {
        isSlashing = true;

        float duration = 0.3f; //duration per slash
        int steps = 30;
        float radius = 2f;
        float angleRange = 180f;

        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 center = player.position + forward * 1.5f;

        //left to right slash
        for (int i = 0; i <= steps; i++)
        {
            if (returningToOrbit) break;  //exit if the orb starts returning to orbit

            float t = (float)i / steps;
            float angle = Mathf.Lerp(-angleRange / 2, angleRange / 2, t);

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 offset = rotation * forward * radius;
            Vector3 arcPos = center + offset + Vector3.up * verticalOffset;

            transform.position = arcPos;

            //update orbit position based on player position
            center = player.position + forward * 1.5f;

            yield return new WaitForSeconds(duration / steps);
        }

        //right to left slash
        for (int i = 0; i <= steps; i++)
        {
            if (returningToOrbit) break;

            float t = (float)i / steps;
            float angle = Mathf.Lerp(angleRange / 2, -angleRange / 2, t);

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 offset = rotation * forward * radius;
            Vector3 arcPos = center + offset + Vector3.up * verticalOffset;

            transform.position = arcPos;

            //update orbit position based on player position
            center = player.position + forward * 1.5f;

            yield return new WaitForSeconds(duration / steps);
        }

        //down to up slash
        Vector3 forwardDir = cameraTransform.forward;
        forwardDir.y = 0;
        forwardDir.Normalize();

        Vector3 right = cameraTransform.right;
        Vector3 baseStart = player.position + forwardDir * 1.5f + Vector3.up * 0.5f; //starts in front and slightly up to avoid clipping with ground
        Vector3 baseEnd = baseStart + Vector3.up * 2f; //ends higher up so that the slash isn't too short

        for (int i = 0; i <= steps; i++)
        {
            if (returningToOrbit) break;

            float t = (float)i / steps;
            Vector3 upwardArc = Vector3.Lerp(baseStart, baseEnd, t);

            //front facing curve
            float outwardOffset = Mathf.Sin(t * Mathf.PI) * 0.75f;
            upwardArc += forwardDir * outwardOffset;

            transform.position = upwardArc;

            //update orbit position based on player position
            baseStart = player.position + forwardDir * 1.5f + Vector3.up * 0.5f;

            yield return new WaitForSeconds(duration / steps);
        }

        //smoothly returns to orbit
        returningToOrbit = true;
        isSlashing = false;
        hasArrived = false;
        enemyTarget = null;
        movingToTarget = false;
        returningToPlayer = false;
    }

    void ReturnToOrbit()
    {
        //figure out current orbit target based on time
        float angle = Time.time * orbitSpeed;
        float x = player.position.x + Mathf.Cos(angle) * orbitRadius;
        float z = player.position.z + Mathf.Sin(angle) * orbitRadius;
        float y = player.position.y + verticalOffset;
        Vector3 desiredOrbitPos = new Vector3(x, y, z);

        transform.position = Vector3.MoveTowards(transform.position, desiredOrbitPos, returnSpeed * Time.deltaTime);

        //snap back to orbit once close enough
        if (Vector3.Distance(transform.position, desiredOrbitPos) < 0.1f)
        {
            returningToOrbit = false;
            hasArrived = false;
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
