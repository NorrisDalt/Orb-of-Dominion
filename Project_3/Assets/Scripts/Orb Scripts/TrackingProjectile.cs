using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;
    public Transform player;
    public Transform cameraTransform;
    public Collider orbCollider;
    public OrbMovement orbMovement;

    private float lastFireTime = -Mathf.Infinity; //start at negative infinity so no cooldown at start
    public float fireCooldown = 5f;

    void Update() 
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastFireTime + fireCooldown) //cooldown to prevent spamming
        {
            ShootProjectile();
            lastFireTime = Time.time; //start cooldown after the first shot
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

    private IEnumerator CheckHitAndMoveOrb(Projectile projectileScript) //move the orb to hit position
    {
        yield return new WaitUntil(() => projectileScript.hasHit);

        orbMovement.SetTargetPosition(projectileScript.hitPosition);
    }

    private IEnumerator DestroyProjectile(GameObject projectile)
    {
        yield return new WaitForSeconds(3f);
        Destroy(projectile);
    }
}
