using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbPortal : MonoBehaviour
{
    public OrbMovement orbMovement;
    public bool abilityUnlocked = false; //unlock when needed
    private StateController controller;
    private float manaCost = 20f;

    void Start()
    {
        controller = FindObjectOfType<StateController>();
        orbMovement =  FindObjectOfType<OrbMovement>();
    }

    void Update()
    {
        if (abilityUnlocked && Input.GetKeyDown(KeyCode.Alpha1) && orbMovement.HasArrived())
        {
            SwapPositions();
        }
        orbMovement =  FindObjectOfType<OrbMovement>(); //Temporary solution
    }

    public void SwapPositions()
    {
        Vector3 tempPosition = transform.position;

        //swap positions
        transform.position = orbMovement.transform.position;
        orbMovement.transform.position = tempPosition;

        //prevent the orb from resuming its orbit immediately
        orbMovement.SetArrivedState(true);

        controller.currentMana -= manaCost;
        controller.manaSlider.value = controller.currentMana;

        
    }
}
