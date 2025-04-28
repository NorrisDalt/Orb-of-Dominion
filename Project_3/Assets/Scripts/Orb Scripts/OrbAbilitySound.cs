using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbAbilitySound : MonoBehaviour
{
    public StateController stateController;
    public SingularityScript gravityScript;
    public OrbDrain drainScript;

    public AudioSource orbAudio;

    public AudioClip gravityClip;
    public AudioClip drainClip;

    private bool gravitySoundWasOn;
    private bool drainSoundWasPlayed;

    public void Start()
    {
        SetupReferences();
    }

    public void Update()
    {
        if (orbAudio == null || gravityScript == null || drainScript == null || stateController == null)
        {
            SetupReferences();
        }

        HandleSound();
    }

    private void SetupReferences()
    {
        if (orbAudio == null)
            orbAudio = GameObject.FindGameObjectWithTag("Orb")?.GetComponent<AudioSource>();

        if (gravityScript == null)
            gravityScript = FindObjectOfType<SingularityScript>();

        if (drainScript == null)
            drainScript = FindObjectOfType<OrbDrain>();

        if (stateController == null)
            stateController = FindObjectOfType<StateController>();
    }

    private void HandleSound()
    {
        if (stateController.gravitySound && !gravitySoundWasOn)
        {
            PlayLoopingClip(gravityClip); // Play the gravity sound on loop
            gravitySoundWasOn = true;
        }
        else if (!stateController.gravitySound && gravitySoundWasOn)
        {
            orbAudio.Stop(); // Stop the gravity sound
            gravitySoundWasOn = false;
        }

        if (drainScript != null && drainScript.isDraining && !drainSoundWasPlayed)
        {
            PlayOneShotClip(drainClip); // Play drain sound when draining starts
            drainSoundWasPlayed = true;
        }
        else if (drainScript != null && !drainScript.isDraining && drainSoundWasPlayed)
        {
            drainSoundWasPlayed = false;  // Reset for the next time draining starts
        }
    }

    private void PlayLoopingClip(AudioClip clip)
    {
        if (orbAudio != null && clip != null)
        {
            if (!orbAudio.isPlaying || orbAudio.clip != clip)
            {
                orbAudio.clip = clip;
                orbAudio.loop = true; // Make it loop
                orbAudio.Play();
            }
        }
    }

    private void PlayOneShotClip(AudioClip clip)
    {
        if (orbAudio != null && clip != null)
        {
            orbAudio.PlayOneShot(clip); 
        }
    }
}
