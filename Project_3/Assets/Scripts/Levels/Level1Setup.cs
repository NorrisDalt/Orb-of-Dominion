using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level1Setup : MonoBehaviour
{
    void Start()
    {
        var player = FindObjectOfType<StateController>();
        if (player != null)
        {
            // Use the public property instead of direct field access
            player.orbMovement = FindObjectOfType<OrbMovement>();
            
            // Additional level-specific setup
            if (player.manaSlider == null)
            {
                player.manaSlider = GameObject.Find("ManaSlider")?.GetComponent<Slider>();
            }
        }
    }
}