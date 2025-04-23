using UnityEngine;

public class SingularityScript : MonoBehaviour
{
    public Rigidbody rb;
    public float G = 6.674f;
    public StateController controller;
    private float manaCost = 0.15f;

    void Awake()
    {
        // Make persistent and ensure only one exists
        SingularityScript[] existingSingularities = FindObjectsOfType<SingularityScript>();
        if (existingSingularities.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        
        // Auto-configure if missing
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (controller == null) controller = FindObjectOfType<StateController>();
    }

    void FixedUpdate()
    {
        if (controller == null) controller = FindObjectOfType<StateController>();
        if (!enabled || controller == null) return;
        
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach(Enemy enemy in enemies)
        {
            if (enemy.rb == null) continue;
            
            Vector3 direction = rb.position - enemy.rb.position;
            float distance = direction.magnitude;
            float forceMagnitude = G * (rb.mass * enemy.rb.mass) / Mathf.Pow(distance, 2);
            enemy.rb.AddForce(direction.normalized * forceMagnitude);
            
            // Safe mana update
            if (controller != null)
            {
                controller.currentMana -= manaCost;
                if (controller.manaSlider != null)
                    controller.manaSlider.value = controller.currentMana;
            }
        }
    }
}