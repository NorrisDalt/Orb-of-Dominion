using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    public float launchForce = 10f;  // Force applied to the player
    public Vector3 launchDirection = Vector3.forward;  // Direction to launch the player

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that enters the trigger is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Get the Rigidbody of the player
            Rigidbody playerRb = other.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // Log message to confirm the player was detected
                Debug.Log("Player detected! Applying force...");

                // Apply an impulse force in the specified direction
                playerRb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);
            }
            else
            {
                // Log if the player doesn't have a Rigidbody attached
                Debug.LogWarning("The player does not have a Rigidbody component attached.");
            }
        }
    }
}
