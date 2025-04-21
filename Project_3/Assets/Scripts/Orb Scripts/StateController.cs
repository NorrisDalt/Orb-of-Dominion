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

    // Settings
    [Header("Settings")]
    public float cooldownDuration = 5f;

    // Game State
    private Dictionary<PlayerAbility, float> cooldowns = new Dictionary<PlayerAbility, float>();
    private int currentLevel = 1;
    private Dictionary<KeyCode, PlayerAbility> abilityHotkeys = new Dictionary<KeyCode, PlayerAbility>();
    private PlayerAbility currentlySelectedAbility = PlayerAbility.None;
    private bool hasSelectedAbility = false;

    public float maxMana;
    private float currentMana;


    public enum PlayerAbility
    {   
        None = 0,
        Tether = 1,
        Portal = 2,
        Homming = 3,
        Singularity = 4,
        Drain = 5
    }

    void Start()
    {
        InitializeComponents();
        StartLevel();
    }

    void Update()
    {
        if (!hasSelectedAbility) return;
        
        CheckHotkeyPresses();
        UpdateCooldowns();
        
        if (Input.GetMouseButtonDown(1)) // Right-click activation
        {
            ActivateCurrentAbility();
        }
    }

    #region Initialization
    private void InitializeComponents()
    {
        tetherAbility = GetComponent<OrbTether>();
        portalAbility = GetComponent<OrbPortal>();
        orbMovement = FindObjectOfType<OrbMovement>();

        foreach (Image ui in abilityUI)
        {
            ui.enabled = false;
        }
        abilitySelectionPanel.SetActive(false);

        // Ensure EventSystem exists
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>();
        }
    }
    #endregion

    #region Hotkey System
    private void CheckHotkeyPresses()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && abilityHotkeys.ContainsKey(KeyCode.Alpha1))
        {
            SelectAbility(abilityHotkeys[KeyCode.Alpha1]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && abilityHotkeys.ContainsKey(KeyCode.Alpha2))
        {
            SelectAbility(abilityHotkeys[KeyCode.Alpha2]);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && abilityHotkeys.ContainsKey(KeyCode.Alpha3))
        {
            SelectAbility(abilityHotkeys[KeyCode.Alpha3]);
        }
    }

    private void SelectAbility(PlayerAbility ability)
    {
        // Disable all abilities first
        if (singularityAbility != null) singularityAbility.enabled = false;
        if (portalAbility != null) portalAbility.enabled = false;
        if (tetherAbility != null) tetherAbility.enabled = false;

        currentlySelectedAbility = ability;
        UpdateAbilityUI();
    }

    private void UpdateAbilityUI()
    {
        // Disable all UI first
        foreach (Image ui in abilityUI)
        {
            if (ui != null) ui.enabled = false;
        }

        // Enable only the selected ability's UI
        switch (currentlySelectedAbility)
        {
            case PlayerAbility.Singularity:
                if (abilityUI.Length > 0 && abilityUI[0] != null) abilityUI[0].enabled = true;
                break;
            case PlayerAbility.Portal:
                if (abilityUI.Length > 1 && abilityUI[1] != null) abilityUI[1].enabled = true;
                break;
            case PlayerAbility.Tether:
                if (abilityUI.Length > 2 && abilityUI[2] != null) abilityUI[2].enabled = true;
                break;
        }
    }

    private void ActivateCurrentAbility()
    {
        if (currentlySelectedAbility == PlayerAbility.None) return;

        switch (currentlySelectedAbility)
        {
            case PlayerAbility.Singularity:
                if (!IsAbilityOnCooldown(PlayerAbility.Singularity) && singularityAbility != null)
                {
                    singularityAbility.enabled = !singularityAbility.enabled;
                    cooldowns[PlayerAbility.Singularity] = cooldownDuration;
                }
                break;

            case PlayerAbility.Portal:
                if (!IsAbilityOnCooldown(PlayerAbility.Portal) && orbMovement != null && orbMovement.HasArrived() && portalAbility != null)
                {
                    portalAbility.SwapPositions();
                    cooldowns[PlayerAbility.Portal] = cooldownDuration;
                }
                break;

            case PlayerAbility.Tether:
                if (!IsAbilityOnCooldown(PlayerAbility.Tether) && tetherAbility != null)
                {
                    tetherAbility.TetherToggle();
                }
                break;
        }
    }
    #endregion

    #region Level Progression
    public void StartLevel()
    {
        hasSelectedAbility = false;
        
        if (currentLevel == 1)
        {
            abilityHotkeys.Clear();
        }

        ShowAbilitySelection();
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
        }
    }
    #endregion

    #region Ability Selection
    private void ShowAbilitySelection()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
        abilitySelectionPanel.SetActive(true);

        List<PlayerAbility> randomAbilities = GetRandomAbilities(3);
        
        for (int i = 0; i < abilitySelectionButtons.Length; i++)
        {
            bool shouldEnable = i < randomAbilities.Count;
            if (abilitySelectionButtons[i] != null)
            {
                abilitySelectionButtons[i].gameObject.SetActive(shouldEnable);
            }

            if (shouldEnable && abilitySelectionButtons[i] != null && i < abilityButtonTexts.Length && abilityButtonTexts[i] != null)
            {
                PlayerAbility ability = randomAbilities[i];
                abilityButtonTexts[i].text = GetAbilityName(ability);
                
                abilitySelectionButtons[i].onClick.RemoveAllListeners();
                abilitySelectionButtons[i].onClick.AddListener(() => OnAbilitySelected(ability));
            }
        }
    }

    private void OnAbilitySelected(PlayerAbility selectedAbility)
    {
        // Assign to hotkey based on level
        KeyCode hotkey = KeyCode.Alpha1;
        if (currentLevel == 2) hotkey = KeyCode.Alpha2;
        else if (currentLevel == 3) hotkey = KeyCode.Alpha3;

        abilityHotkeys[hotkey] = selectedAbility;
        Debug.Log($"Assigned {selectedAbility} to {hotkey}");

        // Auto-select if first ability
        if (currentLevel == 1)
        {
            SelectAbility(selectedAbility);
        }

        // Clean up
        abilitySelectionPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        hasSelectedAbility = true;
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
        foreach (KeyValuePair<KeyCode, PlayerAbility> pair in abilityHotkeys)
        {
            availableAbilities.Remove(pair.Value);
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

    #region Debug
    void OnGUI()
    {
        GUILayout.Label($"Level: {currentLevel}");
        foreach (var pair in abilityHotkeys)
        {
            GUILayout.Label($"{pair.Key}: {pair.Value} {(currentlySelectedAbility == pair.Value ? "(ACTIVE)" : "")}");
        }
    }
    #endregion
}