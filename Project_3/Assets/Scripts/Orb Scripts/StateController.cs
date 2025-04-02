using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    private OrbTether tetherAbility;
    private OrbPortal portalAbility;
    private SingularityScript singularityAbility;
    private OrbMovement orbMovement;
    
    public enum PlayerAbility
    {
        False = 0,
        True = 1,
        Tether = 2,
        Portal = 3,
        Homming = 4,
        Singularity = 5,
        Drain = 6
    }

    void Start()
    {
        tetherAbility = GetComponent<OrbTether>();
        portalAbility = GetComponent<OrbPortal>();
        singularityAbility = FindObjectOfType<SingularityScript>();
        orbMovement = FindObjectOfType<OrbMovement>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentAbility = PlayerAbility.Portal;
            Debug.Log("Portal is active");
             //When player clicks 1 2 3  it should change current ability to number   
        }

        ActivateAbility();
    }

    [SerializeField] private PlayerAbility currentAbility;

     

     private void ActivateAbility()
     {
        switch(currentAbility)
        {
            case PlayerAbility.Singularity:
                //Do stuff
                //singularityAbility.
                break;
            
            case PlayerAbility.Tether:
                tetherAbility.TetherToggle();
                //made add if player click button run script
                break;

            case PlayerAbility.Portal:
                if(Input.GetMouseButtonDown(1) && orbMovement.HasArrived())
                {
                    portalAbility.SwapPositions();
                }
                break;
                
        }
     }


}
