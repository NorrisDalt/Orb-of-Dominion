using UnityEngine;

public class SingularityScript : MonoBehaviour
{
    public Rigidbody rb;
    public float G = 6.674f;
    public StateController controller;
    private float manaCost = 0.15f;

    void Awake()
    {
        if (FindObjectsOfType<SingularityScript>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
       controller = FindObjectOfType<StateController>();
    }
    
    void FixedUpdate()
    {
        rb = GameObject.FindWithTag("Orb").GetComponent<Rigidbody>();
        if (!enabled) return; // Only run when enabled by StateController
        
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach(Enemy enemy in enemies)
        {
            Vector3 direction = rb.position - enemy.rb.position;
            float distance = direction.magnitude;
            float forceMagnitude = G * (rb.mass * enemy.rb.mass) / Mathf.Pow(distance, 2);
            enemy.rb.AddForce(direction.normalized * forceMagnitude);
            controller.currentMana -= manaCost;
            controller.manaSlider.value = controller.currentMana; //Change this
        }
    }
}