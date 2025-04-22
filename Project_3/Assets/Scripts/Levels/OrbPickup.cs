using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPickup : MonoBehaviour
{
    [SerializeField] private GameObject orbModelOnPedestal;
    [SerializeField] private GameObject actualOrbToUnhide;
    [SerializeField] private GameObject doorToDestroy; // ⬅️ New

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            PickupOrb();
        }
    }

    private void PickupOrb()
    {
        if (orbModelOnPedestal != null)
        {
            Destroy(orbModelOnPedestal);
        }

        if (actualOrbToUnhide != null)
        {
            actualOrbToUnhide.SetActive(true);
        }

        if (doorToDestroy != null)
        {
            Destroy(doorToDestroy);
        }

        FindObjectOfType<TutorialManager>()?.OnOrbRetrieved();

        Destroy(gameObject); // remove trigger
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
