using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class StateController : MonoBehaviour
{
    // UI References
    [Header("UI References")]
    public Image[] abilityUI;
    public GameObject abilitySelectionPanel;
    public Button[] abilitySelectionButtons;
    public TextMeshProUGUI[] abilityButtonTexts;

    // Ability Components
    [Header("Ability Components")]
    private OrbTether tetherAbility;
    private OrbPortal portalAbility;
    public SingularityScript singularityAbility;
    private OrbMovement orbMovement;
    private OrbDrain orbDrain;

    // Settings
    [Header("Settings")]
    public float cooldownDuration = 5f;
    public PlayerAbility currentAbility { get; private set; } = PlayerAbility.None;

    // Game State
    private Dictionary<PlayerAbility, float> cooldowns = new Dictionary<PlayerAbility, float>();
    private int currentLevel = 1;
    private PlayerAbility[] selectedAbilities = new PlayerAbility[3];
    private bool hasSelectedAbility = false;
    

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
        InitializeComponents();
        StartLevel();
    }

    void Update()
    {
        if (!hasSelectedAbility) return;
        
        HandleAbilitySwitching();
        UpdateCooldowns();
        ActivateAbility();
    }

    #region Initialization
    private void InitializeComponents()
    {
        tetherAbility = GetComponent<OrbTether>();
        portalAbility = GetComponent<OrbPortal>();
        orbMovement = FindObjectOfType<OrbMovement>();
        orbDrain = GetComponent<OrbDrain>();

        foreach (Image ui in abilityUI)
        {
            ui.enabled = false;
        }
        abilitySelectionPanel.SetActive(false);
    }
    #endregion

    #region Level Progression
    public void StartLevel()
    {
        currentAbility = PlayerAbility.None;
        hasSelectedAbility = false;
        
        if (currentLevel <= 3 && selectedAbilities[currentLevel - 1] != PlayerAbility.None)
        {
            currentAbility = selectedAbilities[currentLevel - 1];
            ActivateSelectedAbility();
            hasSelectedAbility = true;
        }
        else
        {
            ShowAbilitySelection();
        }
    }

    public void AdvanceToNextLevel()
    {
        if (currentLevel < 3)
        {
            currentLevel++;
            StartLevel();
        }
        else
        {
            Debug.Log("Game completed!");
            // Handle game completion
        }
    }
    #endregion

    #region Ability Selection
    private void ShowAbilitySelection()
    {
        // Unlock and show mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventSystem.current.SetSelectedGameObject(null);
        abilitySelectionButtons[0].Select();

        Time.timeScale = 0f;
        abilitySelectionPanel.SetActive(true);
        
        List<PlayerAbility> randomAbilities = GetRandomAbilities(3);
        
        for (int i = 0; i < abilitySelectionButtons.Length; i++)
        {
            bool shouldEnable = i < randomAbilities.Count;
            abilitySelectionButtons[i].gameObject.SetActive(shouldEnable);

            if (shouldEnable)
            {
                PlayerAbility ability = randomAbilities[i];
                abilityButtonTexts[i].text = GetAbilityName(ability);
                
                abilitySelectionButtons[i].onClick.RemoveAllListeners();
                abilitySelectionButtons[i].onClick.AddListener(() => OnAbilitySelected(ability));
            }
        }
    }

    private List<PlayerAbility> GetRandomAbilities(int count)
    {
        List<PlayerAbility> availableAbilities = new List<PlayerAbility>
        {
            PlayerAbility.Tether,
            PlayerAbility.Portal,
            PlayerAbility.Singularity,
            PlayerAbility.Homming,
            PlayerAbility.Drain
        };

        // Remove already selected abilities
        for (int i = 0; i < currentLevel - 1; i++)
        {
            availableAbilities.Remove(selectedAbilities[i]);
        }

        // Fisher-Yates shuffle
        for (int i = availableAbilities.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            PlayerAbility temp = availableAbilities[i];
            availableAbilities[i] = availableAbilities[randomIndex];
            availableAbilities[randomIndex] = temp;
        }

        return availableAbilities.GetRange(0, Mathf.Min(count, availableAbilities.Count));
    }

    private void OnAbilitySelected(PlayerAbility selectedAbility)
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        selectedAbilities[currentLevel - 1] = selectedAbility;
        abilitySelectionPanel.SetActive(false);
        Time.timeScale = 1f;
        currentAbility = selectedAbility;
        hasSelectedAbility = true;
        ActivateSelectedAbility();
    }

    private string GetAbilityName(PlayerAbility ability)
    {
        return ability switch
        {
            PlayerAbility.Tether => "Tether",
            PlayerAbility.Portal => "Portal",
            PlayerAbility.Singularity => "Singularity",
            PlayerAbility.Homming => "Homming",
            PlayerAbility.Drain => "Drain",
            _ => ability.ToString()
        };
    }
    #endregion

    #region Ability Management
    private void ActivateSelectedAbility()
    {
        foreach (Image ui in abilityUI) ui.enabled = false;

        switch (currentAbility)
        {
            case PlayerAbility.Portal:
                SetCurrentAbility(PlayerAbility.Portal, 0);
                break;
            case PlayerAbility.Singularity:
                SetCurrentAbility(PlayerAbility.Singularity, 1);
                break;
            case PlayerAbility.Tether:
                SetCurrentAbility(PlayerAbility.Tether, 2);
                break;
            case PlayerAbility.Homming:
                SetCurrentAbility(PlayerAbility.Homming, 3);
                break;
            case PlayerAbility.Drain:
                SetCurrentAbility(PlayerAbility.Drain, -1);
                break;
        }
    }

    private void SetCurrentAbility(PlayerAbility ability, int uiIndex)
    {
        if (uiIndex >= 0 && uiIndex < abilityUI.Length)
        {
            abilityUI[uiIndex].enabled = true;
        }
        currentAbility = ability;
        Debug.Log($"{ability} activated for Level {currentLevel}");
    }
    #endregion

    #region Ability Activation
    private void HandleAbilitySwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetCurrentAbility(PlayerAbility.Portal, 0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SetCurrentAbility(PlayerAbility.Singularity, 1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SetCurrentAbility(PlayerAbility.Tether, 2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SetCurrentAbility(PlayerAbility.Homming, 3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) SetCurrentAbility(PlayerAbility.Drain, -1);
    }

    private void ActivateAbility()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        switch (currentAbility)
        {
            case PlayerAbility.Singularity:
                if (!IsAbilityOnCooldown(PlayerAbility.Singularity))
                {
                    singularityAbility.enabled = !singularityAbility.enabled;
                    cooldowns[PlayerAbility.Singularity] = cooldownDuration;
                }
                break;

            case PlayerAbility.Tether:
                tetherAbility.TetherToggle();
                break;

            case PlayerAbility.Portal:
                if (orbMovement.HasArrived() && !IsAbilityOnCooldown(PlayerAbility.Portal))
                {
                    portalAbility.SwapPositions();
                    cooldowns[PlayerAbility.Portal] = cooldownDuration;
                }
                break;

            case PlayerAbility.Homming:
                if (orbMovement.enemyTarget != null && !orbMovement.movingToTarget)
                {
                    orbMovement.FollowEnemy();
                }
                break;
        }
    }
    #endregion

    #region Cooldown System
    private void UpdateCooldowns()
    {
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
        return ability switch
        {
            PlayerAbility.Tether => false,
            PlayerAbility.Homming => false,
            _ => cooldowns.ContainsKey(ability)
        };
    }
    #endregion
    public void DebugClick()
{
    Debug.Log("Button clicked!");
}
}