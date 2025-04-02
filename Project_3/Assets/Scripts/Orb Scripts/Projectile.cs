using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 hitPosition; //store the hit position
    public Transform enemyTarget; //store reference to the enemy
    public bool hasHit = false;
    public bool isHoming = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;

            if (collision.gameObject.CompareTag("Enemy"))
            {
                isHoming = true;
                enemyTarget = collision.transform; //assigns the enemy as the target
            }
            else
            {
                hitPosition = collision.contacts[0].point; //stores floor hit position
            }

            Destroy(gameObject); //destroys the tracking projectile
        }
    }
}
