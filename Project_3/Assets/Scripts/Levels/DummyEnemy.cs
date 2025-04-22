using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : MonoBehaviour
{
    [SerializeField] private int hitsToDestroy = 3;
    [SerializeField] private float hitCooldown = 0.2f; // time between registered hits

    private int currentHits = 0;
    private float lastHitTime = -999f;

    public delegate void DummyDestroyed();
    public static event DummyDestroyed OnDummyDestroyed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb") && Time.time > lastHitTime + hitCooldown)
        {
            lastHitTime = Time.time;
            currentHits++;

            if (currentHits >= hitsToDestroy)
            {
                OnDummyDestroyed?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
