using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OrbPickup : MonoBehaviour
{
    [SerializeField] private GameObject orbModelOnPedestal;
    [SerializeField] private GameObject actualOrbToUnhide;
    [SerializeField] private GameObject doorToDestroy;

    [SerializeField] private InputActionReference interactAction; // ← Assign "Interact" here in Inspector

    private bool playerInRange = false;

    private void OnEnable()
    {
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange)
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
