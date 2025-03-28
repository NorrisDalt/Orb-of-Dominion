using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Vector3 target;
    public float step = 1f;
    public bool isFollow = false; 
    public GameObject currentProjectile; 
    private Projectile projectileScript;
    
    void Update()
    {
        currentProjectile = GameObject.FindGameObjectWithTag("Projectile");

        if(currentProjectile != null && projectileScript == null)
        {
            projectileScript = currentProjectile.GetComponent<Projectile>();
        }

        if(projectileScript != null)
        {
            // Directly use the Vector3 - no .position needed
            target = projectileScript.homingPosition;
        }
        
        //Go();
    }

    void Go()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }
}