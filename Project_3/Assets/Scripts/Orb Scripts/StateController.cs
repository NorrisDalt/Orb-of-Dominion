using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StateController : MonoBehaviour
{
    // Abilities
    private OrbTether tetherAbility;
    private OrbPortal portalAbility;
    public SingularityScript singularityAbility;
    private OrbMovement orbMovement;

    // UI
    public Image[] abilityUI;
    private Dictionary<PlayerAbility, float> cooldowns = new Dictionary<PlayerAbility, float>();

    // Settings
    public float cooldownDuration = 5f;
    [SerializeField] public PlayerAbility currentAbility = PlayerAbility.None;

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
        orbMovement = FindObjectOfType<OrbMovement>();

        // Initialize all ability UIs as disabled
        foreach (Image ui in abilityUI)
        {
            ui.enabled = false;
        }
    }

    void Update()
    {
        HandleAbilitySwitching();
        UpdateCooldowns();
        ActivateAbility();
    }

    private void HandleAbilitySwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) 
        {
            SetCurrentAbility(PlayerAbility.Portal, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCurrentAbility(PlayerAbility.Singularity, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetCurrentAbility(PlayerAbility.Tether, 2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetCurrentAbility(PlayerAbility.Homming, 3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SetCurrentAbility(PlayerAbility.Drain, -1); // No UI for Drain?
        }
    }

    private void SetCurrentAbility(PlayerAbility ability, int uiIndex)
    {
        // Disable all UI first
        foreach (Image ui in abilityUI)
        {
            ui.enabled = false;
        }

        // Enable only the selected ability's UI
        if (uiIndex >= 0 && uiIndex < abilityUI.Length)
        {
            abilityUI[uiIndex].enabled = true;
        }

        currentAbility = ability;
        Debug.Log(ability + " is active");
    }

    private void UpdateCooldowns()
    {
        // Decrease all active cooldowns
        List<PlayerAbility> keys = new List<PlayerAbility>(cooldowns.Keys);
        foreach (var ability in keys)
        {
            cooldowns[ability] -= Time.deltaTime;
            if (cooldowns[ability] <= 0)
            {
                cooldowns.Remove(ability);
            }
        }
    }

    private bool IsAbilityOnCooldown(PlayerAbility ability)
    {
        // Tether and Homming ignore cooldowns
        if (ability == PlayerAbility.Tether || ability == PlayerAbility.Homming)
            return false;
            
        return cooldowns.ContainsKey(ability);
    }

    private void ActivateAbility()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click activation
        {
            switch (currentAbility)
            {
                case PlayerAbility.Singularity:
                    if (!IsAbilityOnCooldown(PlayerAbility.Singularity))
                    {
                        singularityAbility.enabled = !singularityAbility.enabled;
                        cooldowns.Add(PlayerAbility.Singularity, cooldownDuration);
                    }
                    break;

                case PlayerAbility.Tether:
                    // No cooldown check for Tether
                    tetherAbility.TetherToggle();
                    break;

                case PlayerAbility.Portal:
                    if (orbMovement.HasArrived() && !IsAbilityOnCooldown(PlayerAbility.Portal))
                    {
                        portalAbility.SwapPositions();
                        cooldowns.Add(PlayerAbility.Portal, cooldownDuration);
                    }
                    break;

                case PlayerAbility.Homming:
                    // No cooldown check for Homming
                    if (orbMovement.enemyTarget != null && !orbMovement.movingToTarget == false)
                    {
                        orbMovement.FollowEnemy();
                    }
                    break;
            }
        }
    }
}   