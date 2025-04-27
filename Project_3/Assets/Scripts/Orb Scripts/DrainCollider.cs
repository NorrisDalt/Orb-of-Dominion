using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainCollider : MonoBehaviour
{
    private OrbDrain orbDrain;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            orbDrain = playerObj.GetComponent<OrbDrain>();
        }

        if (orbDrain == null)
        {
            Debug.LogWarning("OrbDrain not found on Player for DrainCollider.");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (orbDrain != null)
        {
            orbDrain.CheckDrainTrigger(other);
        }
    }
}