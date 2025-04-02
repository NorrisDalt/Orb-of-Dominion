using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour
{
    private OrbTether tetherAbility;
    private OrbPortal portalAbility;
    public SingularityScript singularityAbility;
    private OrbMovement orbMovement;
    
    public enum PlayerAbility
    {
        None = 0,
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
        //singularityAbility = FindObjectOfType<SingularityScript>();
        orbMovement = FindObjectOfType<OrbMovement>();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) //When player clicking one, portal ability can now be used
        {
            currentAbility = PlayerAbility.Portal;
            Debug.Log("Portal is active"); 
             //When player clicks 1 2 3  it should change current ability to number   
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))//When player clicking two, Singularity ability can now be used
        {
            currentAbility = PlayerAbility.Singularity;
            Debug.Log("Singularity is active");
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))//When player clicking 3, tether ability can now be used
        {
            currentAbility = PlayerAbility.Tether;
            Debug.Log("Tether is active");
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))//When player clicking 3, tether ability can now be used
        {
            currentAbility = PlayerAbility.Homming;
            Debug.Log("Homming is active");
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))//When player clicking 3, tether ability can now be used
        {
            currentAbility = PlayerAbility.Drain;
            Debug.Log("Drain is active");
        }
        //Need to add cooldowns and make other abilities turn off once currentAbility state swaps.


        ActivateAbility();
    }

    [SerializeField] private PlayerAbility currentAbility;

     

     private void ActivateAbility()
     {
        switch(currentAbility)
        {
            case PlayerAbility.Singularity:
                if(Input.GetMouseButtonDown(1))
                {
                    singularityAbility.enabled = !singularityAbility.enabled;
                }
                if(currentAbility != PlayerAbility.Singularity)
                {
                    singularityAbility.enabled = false;
                }
                break;
            
            case PlayerAbility.Tether:
                if(Input.GetMouseButtonDown(1))
                {
                    tetherAbility.TetherToggle();
                }
                break;

            case PlayerAbility.Portal:
                if(Input.GetMouseButtonDown(1) && orbMovement.HasArrived())
                {
                    portalAbility.SwapPositions();
                }
                break;
            case PlayerAbility.Homming:
                if(orbMovement.enemyTarget != null && orbMovement.movingToTarget == false)
                {
                        orbMovement.FollowEnemy();
                }
                break;
            
                
        }
     }


}
