using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 hitPosition; //store the hit position
    public bool hasHit = false; //flag to check if it’s hit something

    void OnCollisionEnter(Collision collision)
    {
        if (!hasHit)
        {
            hasHit = true;
            hitPosition = collision.contacts[0].point; //get the collision point
            Destroy(gameObject); //destroy the projectile after hit
        }
    }
}
