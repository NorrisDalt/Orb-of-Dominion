using UnityEngine;

public class SingularityScript : MonoBehaviour
{
    public Rigidbody rb;
    public float G = 6.674f;
    
    void FixedUpdate()
    {
        if (!enabled) return; // Only run when enabled by StateController
        
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach(Enemy enemy in enemies)
        {
            Vector3 direction = rb.position - enemy.rb.position;
            float distance = direction.magnitude;
            float forceMagnitude = G * (rb.mass * enemy.rb.mass) / Mathf.Pow(distance, 2);
            enemy.rb.AddForce(direction.normalized * forceMagnitude);
        }
    }
}