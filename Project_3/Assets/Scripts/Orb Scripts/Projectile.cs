using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 hitPosition; //store the hit position
    public Vector3 homingPosition; 
    public bool hasHit = false; //flag to check if it�s hit something

    public bool isHoming = true;

    void OnCollisionEnter(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;
            hitPosition = collision.contacts[0].point; //get the collision point
            Destroy(gameObject); //destroy the projectile after hit
        }

        if(isHoming == true && collision.gameObject.CompareTag("Enemy") && !hasHit)
        {
            hasHit = true;
            homingPosition = collision.contacts[0].point;
            Destroy(gameObject);
        }

        
    }
}
