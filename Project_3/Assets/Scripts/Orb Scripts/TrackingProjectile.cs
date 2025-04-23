using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackingProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public Transform player;
    public Transform cameraTransform;
    public Collider orbCollider;
    public OrbMovement orbMovement;

    private float lastFireTime = -Mathf.Infinity; //makes it so that the projectile doesnt start with a cooldown
    public float fireCooldown = 5f;

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        cameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame) && Time.time >= lastFireTime + fireCooldown) //cooldown to prevent spamming
        {
            ShootProjectile();
            lastFireTime = Time.time; //starts the cooldown after the first shot
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab != null && player != null && cameraTransform != null)
        {
            Vector3 spawnPosition = player.position + cameraTransform.forward * 1f + Vector3.up * 1.0f; //spawning the projectile
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

            StartCoroutine(DestroyProjectile(projectile));

            Vector3 shootDirection = cameraTransform.forward; //finding what direction to send it
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.velocity = shootDirection * projectileSpeed; //shooting the projectile
            }

            Collider projectileCollider = projectile.GetComponent<Collider>(); //ignore the orb and player colliders
            if (projectileCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, player.GetComponent<Collider>());
                if (orbCollider != null)
                {
                    Physics.IgnoreCollision(projectileCollider, orbCollider);
                }
            }

            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                StartCoroutine(CheckHitAndMoveOrb(projectileScript));
            }
        }
    }

    private IEnumerator CheckHitAndMoveOrb(Projectile projectileScript) //move the orb to hit position or enemy
    {
        yield return new WaitUntil(() => projectileScript.hasHit);

        if (projectileScript.enemyTarget != null) //if the projectile hit an enemy
        {
            orbMovement.SetTargetPosition(Vector3.zero, projectileScript.enemyTarget);
        }
        else //if the projectile hit the ground
        {
            orbMovement.SetTargetPosition(projectileScript.hitPosition, null);
        }
    }

    private IEnumerator DestroyProjectile(GameObject projectile) //destroys projectile after 3 seconds
    {
        yield return new WaitForSeconds(3f);
        Destroy(projectile);
    }
}
