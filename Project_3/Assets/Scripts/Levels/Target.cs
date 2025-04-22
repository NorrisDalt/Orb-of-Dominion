using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private GameObject doorToDestroy;
    [SerializeField] private GameObject barrierToDestroy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Orb")) // Only reacts to the orb
        {
            if (doorToDestroy != null)
                Destroy(doorToDestroy);

            if (barrierToDestroy != null)
                Destroy(barrierToDestroy);

            FindObjectOfType<TutorialManager>()?.OnTargetHit();

            Destroy(gameObject); // Optional: destroy the target after it's hit
        }
    }
}
