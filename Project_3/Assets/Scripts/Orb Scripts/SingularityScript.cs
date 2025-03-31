using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingularityScript : MonoBehaviour
{
    public Rigidbody rb;

    public float G = 6.674f;

    void FixedUpdate()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach(Enemy enemy in enemies)
        {
            Attract(enemy);
        }
    }

   void Attract(Enemy objToAttract)
   {
        Rigidbody rbToAttract = objToAttract.rb;

        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;

        float forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude; 

        rbToAttract.AddForce(force);
   }
}
