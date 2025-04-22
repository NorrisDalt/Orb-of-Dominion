using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;

public class StateController : MonoBehaviour
{
    [Header("UI References")]
    public Image[] abilityUI;
    public GameObject abilitySelectionPanel;
    public Button[] abilitySelectionButtons;
    public TextMeshProUGUI[] abilityButtonTexts;

    [Header("Ability Components")]
    private OrbTether tetherAbility;
    private OrbPortal portalAbility;
    public SingularityScript singularityAbility;
    private OrbMovement orbMovement;
    public OrbDrain drainAbility;

    [Header("Settings")]
    public float cooldownDuration = 5f;
    public Slider manaSlider;
    public float maxMana = 100f;
    public float currentMana;

    // Game State
    private Dictionary<KeyCode, PlayerAbility> abilityHotkeys = new Dictionary<KeyCode, PlayerAbility>();
    private HashSet<PlayerAbility> unlockedAbilities = new HashSet<PlayerAbility>();
    private Dictionary<PlayerAbility, float> cooldowns = new Dictionary<PlayerAbility, float>();
    private PlayerAbility currentlySelectedAbility = PlayerAbility.None;
    private bool hasSelectedAbility = false;

    public enum PlayerAbility
    {   
        None = 0,
        Tether = 1,
        Portal = 2,
        Homming = 3,
        Singularity = 4,
        Drain = 5
    }

    void Awake()
    {
        // Singleton pattern
        if (FindObjectsOfType<StateController>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        InitializeComponents();
        SceneManager.sceneLoaded += OnSceneLoaded;

        currentMana = maxMana;
        manaSlider.maxValue = maxMana;
        manaSlider.value = currentMana;

        HandleAbilitySelectionOnSceneLoad();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeComponents();
        PlacePlayerInNewScene();
        HandleAbilitySelectionOnSceneLoad();
    }

    void HandleAbilitySelectionOnSceneLoad()
    {
        if (IsAbilitySelectionScene() && CanSelectNewAbility())
        {
            StartLevel();
        }
        else if (unlockedAbilities.Count > 0)
        {
            SelectAbility(unlockedAbilities.First());
            hasSelectedAbility = true;
        }
    }

    bool IsAbilitySelectionScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName.Contains("LevelUp") || sceneName.Contains("AbilitySelection");

    }

    bool CanSelectNewAbility()
    {
        return unlockedAbilities.Count < System.Enum.GetValues(typeof(PlayerAbility)).Length - 1;
    }

    void PlacePlayerInNewScene()
    {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }
    }

    void Update()
    {
        if (!hasSelectedAbility) return;
        
        CheckHotkeyPresses();
        UpdateCooldowns();
        
        if (Input.GetMouseButtonDown(1))
        {
            ActivateCurrentAbility();
        }

        if (singularityAbility != null && singularityAbility.enabled && currentMana <= 0)
        {
            singularityAbility.enabled = false;
        }

        if (drainAbility != null && drainAbility.enabled && currentMana <= 0)
        {
            drainAbility.enabled = false;
        }
    }

    #region Core Systems
    private void InitializeComponents()
    {
        tetherAbility = GetComponent<OrbTether>();
        portalAbility = GetComponent<OrbPortal>();
        orbMovement = FindObjectOfType<OrbMovement>();
        drainAbility = GetComponent<OrbDrain>();

        // Find in-scene objects
        orbMovement = FindObjectOfType<OrbMovement>(true); // Include inactive
        // UI - Find by names/paths if not assigned
        if (abilitySelectionPanel == null)
        abilitySelectionPanel = GameObject.Find("Ability Panel");

        if (manaSlider == null)
        manaSlider = GameObject.Find("Mana").GetComponent<Slider>();

        // Buttons - Find by parent
        if (abilitySelectionButtons == null || abilitySelectionButtons.Length == 0)
        {
            Transform buttonParent = abilitySelectionPanel.transform.Find("Buttons");
            abilitySelectionButtons = buttonParent.GetComponentsInChildren<Button>();
        }

        foreach (Image ui in abilityUI)
        {
            ui.enabled = false;
        }
        abilitySelectionPanel.SetActive(false);

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>();
        }
    }

    private void CheckHotkeyPresses()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TrySelectAbility(KeyCode.Alpha1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TrySelectAbility(KeyCode.Alpha2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TrySelectAbility(KeyCode.Alpha3);
    }

    private void TrySelectAbility(KeyCode key)
    {
        if (abilityHotkeys.TryGetValue(key, out PlayerAbility ability))
        {
            SelectAbility(ability);
        }
    }

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
    #endregion

    #region Ability Management
    public void StartLevel()
    {
        hasSelectedAbility = false;
        ShowAbilitySelection();
    }

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

    private void OnAbilitySelected(PlayerAbility selectedAbility)
    {
        KeyCode hotkey = GetNextAvailableHotkey();
        abilityHotkeys[hotkey] = selectedAbility;
        unlockedAbilities.Add(selectedAbility);

        abilitySelectionPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        hasSelectedAbility = true;
        SelectAbility(selectedAbility);
    }

    private KeyCode GetNextAvailableHotkey()
    {
        if (!abilityHotkeys.ContainsKey(KeyCode.Alpha1)) return KeyCode.Alpha1;
        if (!abilityHotkeys.ContainsKey(KeyCode.Alpha2)) return KeyCode.Alpha2;
        return KeyCode.Alpha3;
    }

    private List<PlayerAbility> GetRandomAbilities(int count)
    {
        List<PlayerAbility> allAbilities = System.Enum.GetValues(typeof(PlayerAbility))
            .Cast<PlayerAbility>()
            .Where(a => a != PlayerAbility.None)
            .ToList();

        return allAbilities
            .Where(ability => !unlockedAbilities.Contains(ability))
            .OrderBy(x => Random.value)
            .Take(count)
            .ToList();
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

    private void SelectAbility(PlayerAbility ability)
    {
        // Disable all abilities first
        if (singularityAbility != null) singularityAbility.enabled = false;
        if (portalAbility != null) portalAbility.enabled = false;
        if (tetherAbility != null) tetherAbility.enabled = false;
        if (drainAbility != null) drainAbility.enabled = false;

        currentlySelectedAbility = ability;
        UpdateAbilityUI();
    }

    private void UpdateAbilityUI()
    {
        foreach (Image ui in abilityUI)
        {
            if (ui != null) ui.enabled = false;
        }

        switch (currentlySelectedAbility)
        {
            case PlayerAbility.Singularity:
                if (abilityUI.Length > 0) abilityUI[0].enabled = true;
                break;
            case PlayerAbility.Portal:
                if (abilityUI.Length > 1) abilityUI[1].enabled = true;
                break;
            case PlayerAbility.Tether:
                if (abilityUI.Length > 2) abilityUI[2].enabled = true;
                break;
            case PlayerAbility.Drain:
                if (abilityUI.Length > 3) abilityUI[3].enabled = true; // Assuming index 3 is for Drain
                break;
        }
    }

    private void ActivateCurrentAbility()
    {
        if (currentlySelectedAbility == PlayerAbility.None) return;

        switch (currentlySelectedAbility)
        {
            case PlayerAbility.Singularity:
                if (!IsAbilityOnCooldown(PlayerAbility.Singularity) && currentMana > 0)
                {
                    singularityAbility.enabled = !singularityAbility.enabled;
                    cooldowns[PlayerAbility.Singularity] = cooldownDuration;
                }
                break;

            case PlayerAbility.Portal:
                if (!IsAbilityOnCooldown(PlayerAbility.Portal) && orbMovement.HasArrived() && currentMana > 0)
                {
                    portalAbility.SwapPositions();
                    cooldowns[PlayerAbility.Portal] = cooldownDuration;
                }
                break;

            case PlayerAbility.Tether:
                if (!IsAbilityOnCooldown(PlayerAbility.Tether) && currentMana > 0)
                {
                    tetherAbility.TetherToggle();
                    if (currentMana <= 0 && tetherAbility.isTethered) 
                    {
                        tetherAbility.TetherToggle();
                    }
                }
                break;
            case PlayerAbility.Drain:
                if (!IsAbilityOnCooldown(PlayerAbility.Drain) && currentMana > 0)
                {
                    drainAbility.enabled = !drainAbility.enabled;
                    cooldowns[PlayerAbility.Drain] = cooldownDuration;

                     // Immediately stop if no mana
                    if (currentMana <= 0)
                    {
                        drainAbility.enabled = false;
                    }
                }
                break;
        }
    }

    private bool IsAbilityOnCooldown(PlayerAbility ability)
    {
        return cooldowns.ContainsKey(ability);
    }
    #endregion
}