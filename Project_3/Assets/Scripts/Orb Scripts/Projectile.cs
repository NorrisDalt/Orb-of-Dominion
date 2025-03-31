using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 hitPosition; //store the hit position
    public GameObject homingPosition; 
    public bool hasHit = false; //flag to check if it�s hit something

    private bool isHoming = true;

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
            homingPosition = collision.gameObject;
            Destroy(gameObject);
        }

        
    }
}
