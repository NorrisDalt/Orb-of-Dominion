using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.InputSystem;

public class StateController : MonoBehaviour
{
    [Header("UI References")]
    public Image[] abilityUI;
    public GameObject abilitySelectionPanel;
    public GameObject abilityIconPanel;
    public Button[] abilitySelectionButtons;
    public TextMeshProUGUI[] abilityButtonTexts;
    public GameObject[] abilitySlots;

    [Header("Ability Components")]
    [SerializeField] private OrbTether tetherAbility;
    [SerializeField] private OrbPortal portalAbility;
    public SingularityScript singularityAbility;
    public OrbDrain drainAbility;
    [SerializeField] private OrbMovement _orbMovement;

    // Public property for orbMovement access
    public OrbMovement orbMovement
    {
        get => _orbMovement;
        set => _orbMovement = value;
    }

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

    private PlayerInput playerInput;
    private PlayerMovement playerMovement;

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
        playerInput = GetComponent<PlayerInput>();

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
        
        /*currentMana = maxMana;
        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = currentMana;
        }*/ // change if this doenst work 

        StartCoroutine(DelayedReferenceCheck());
    }

    IEnumerator DelayedReferenceCheck()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (_orbMovement == null)
        {
            _orbMovement = FindObjectOfType<OrbMovement>(true);
            Debug.Log(_orbMovement != null ? "Found OrbMovement" : "OrbMovement missing");
        }
        
      /*  if (manaSlider == null)
        {
            var sliderObj = GameObject.Find("ManaSlider");
            if (sliderObj != null) 
            {
                manaSlider = sliderObj.GetComponent<Slider>();
                if (manaSlider != null)
                {
                    manaSlider.maxValue = maxMana;
                    manaSlider.value = currentMana;
                }
            }
        }*/
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentMana = maxMana;
        //playerMovement.pCurrentHealth = playerMovement.pMaxHealth; 
        StopAllCoroutines();
        InitializeComponents();
        PlacePlayerInNewScene();
        StartCoroutine(DelayedSceneSetup());
        ReferenceAbility();
    }

    IEnumerator DelayedSceneSetup()
    {
        yield return new WaitForEndOfFrame();
        HandleAbilitySelectionOnSceneLoad();
    }

    void PlacePlayerInNewScene()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (spawnPoints.Length > 0)
        {
            transform.position = spawnPoints[0].transform.position;
            transform.rotation = spawnPoints[0].transform.rotation;
        }
        else
        {
            transform.position = Vector3.zero;
            Debug.LogWarning("No spawn points found - using default position");
        }
    }

    void InitializeComponents()
    {
        // Get ability components
        tetherAbility = GetComponent<OrbTether>();
        portalAbility = GetComponent<OrbPortal>();
        drainAbility = GetComponent<OrbDrain>();
        singularityAbility = GetComponent<SingularityScript>();
       // if (singularityAbility == null)
         //   singularityAbility = GetComponent<SingularityScript>();
       // if (drainAbility == null)
        //    drainAbility = GetComponent<OrbDrain>();
        
        // Find OrbMovement in scene
        if (_orbMovement == null)
            _orbMovement = FindObjectOfType<OrbMovement>(true);

        // Initialize UI
        if (abilityUI != null)
        {
            foreach (Image ui in abilityUI)
            {
                if (ui != null) ui.enabled = false;
            }
        }

        if (abilitySelectionPanel != null)
            abilitySelectionPanel.SetActive(false);

        // Ensure EventSystem exists
        if (FindObjectOfType<EventSystem>() == null)
        {
            CreateEventSystem();
        }
    }

    void CreateEventSystem()
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
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

    public void ReferenceAbility()
    {
        abilitySelectionPanel = GameObject.Find("AbilityPanel");
        
        abilityIconPanel = GameObject.Find("AbilityLabelSlots");

        abilitySelectionButtons = abilitySelectionPanel.GetComponentsInChildren<Button>();

        abilityButtonTexts = new TextMeshProUGUI[abilitySelectionButtons.Length];

        for (int i = 0; i < abilitySelectionButtons.Length; i++)
        {
            abilityButtonTexts[i] = abilitySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
        }
       
       manaSlider = GameObject.Find("Mana").GetComponent<Slider>();

       //singularityAbility = FindObjectOfType<SingularityScript>();
     //  drainAbility = FindObjectOfType<OrbDrain>();

    }

    void Update()
    {

        if (!hasSelectedAbility) return;
        
        CheckHotkeyPresses();
        UpdateCooldowns();

        if (playerInput.actions["Use Ability"].WasPressedThisFrame())
        {
            ActivateCurrentAbility();
        }

        //currentMana = maxMana;
        if (manaSlider != null)
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = currentMana;
        } // maybe worked? 

        // Disable abilities if out of mana
        if (singularityAbility != null && singularityAbility.enabled && currentMana <= 0)
            singularityAbility.enabled = false;
        
        if (drainAbility != null && drainAbility.enabled && currentMana <= 0)
            drainAbility.enabled = false;
    }

    void CheckHotkeyPresses()
    {
        var actions = playerInput.actions;

        if (actions["Ability 1"].WasPressedThisFrame() && abilityHotkeys.ContainsKey(KeyCode.Alpha1))
            SelectAbility(abilityHotkeys[KeyCode.Alpha1]);
        else if (actions["Ability 2"].WasPressedThisFrame() && abilityHotkeys.ContainsKey(KeyCode.Alpha2))
            SelectAbility(abilityHotkeys[KeyCode.Alpha2]);
        else if (actions["Ability 3"].WasPressedThisFrame() && abilityHotkeys.ContainsKey(KeyCode.Alpha3))
            SelectAbility(abilityHotkeys[KeyCode.Alpha3]);
    }

    void UpdateCooldowns()
    {
        List<PlayerAbility> keys = new List<PlayerAbility>(cooldowns.Keys);
        foreach (var ability in keys)
        {
            cooldowns[ability] -= Time.deltaTime;
            if (cooldowns[ability] <= 0)
                cooldowns.Remove(ability);
        }
    }

    public void StartLevel()
    {
        hasSelectedAbility = false;
        ShowAbilitySelection();
    }

    void ShowAbilitySelection()
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

    void OnAbilitySelected(PlayerAbility selectedAbility)
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

    KeyCode GetNextAvailableHotkey()
    {
        if (!abilityHotkeys.ContainsKey(KeyCode.Alpha1)) return KeyCode.Alpha1;
        if (!abilityHotkeys.ContainsKey(KeyCode.Alpha2)) return KeyCode.Alpha2;
        return KeyCode.Alpha3;
    }

    List<PlayerAbility> GetRandomAbilities(int count)
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

    string GetAbilityName(PlayerAbility ability)
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

    void SelectAbility(PlayerAbility ability)
    {
        // Disable all abilities first
        if (singularityAbility != null) singularityAbility.enabled = false;
        if (portalAbility != null) portalAbility.enabled = false;
        if (tetherAbility != null) tetherAbility.enabled = false;
        if (drainAbility != null) drainAbility.enabled = false;

        currentlySelectedAbility = ability;
        UpdateAbilityUI();
    }

    void UpdateAbilityUI()
    {
        if (abilityUI == null) return;

        for (int i = 0; i < abilityUI.Length; i++)
        {
            if (abilityUI[i] != null)
                abilityUI[i].enabled = false;
        }

        switch (currentlySelectedAbility)
        {
            case PlayerAbility.Singularity:
                if (abilityUI.Length > 0 && abilityUI[0] != null) 
                    abilityUI[0].enabled = true;
                break;
            case PlayerAbility.Portal:
                if (abilityUI.Length > 1 && abilityUI[1] != null) 
                    abilityUI[1].enabled = true;
                break;
            case PlayerAbility.Tether:
                if (abilityUI.Length > 2 && abilityUI[2] != null) 
                    abilityUI[2].enabled = true;
                break;
            case PlayerAbility.Drain:
                if (abilityUI.Length > 3 && abilityUI[3] != null) 
                    abilityUI[3].enabled = true;
                break;
            case PlayerAbility.Homming:
                if (abilityUI.Length > 4 && abilityUI[4] != null) 
                    abilityUI[4].enabled = true;
                break;
        }
    }

    void ActivateCurrentAbility()
    {
        if (currentlySelectedAbility == PlayerAbility.None || currentMana <= 0) 
            return;

        switch (currentlySelectedAbility)
        {
            case PlayerAbility.Singularity:
                if (singularityAbility != null && !singularityAbility.Equals(null) && !IsAbilityOnCooldown(PlayerAbility.Singularity))
                {
                    singularityAbility.enabled = !singularityAbility.enabled;
                    cooldowns[PlayerAbility.Singularity] = cooldownDuration;
                    //Activate Singularity UI 
                }
        break;

            case PlayerAbility.Portal:
                if (!IsAbilityOnCooldown(PlayerAbility.Portal) && _orbMovement != null && _orbMovement.HasArrived())
                {
                    portalAbility.SwapPositions();
                    cooldowns[PlayerAbility.Portal] = cooldownDuration;
                    //Activate Portal UI
                }
                break;

            case PlayerAbility.Tether:
                if (!IsAbilityOnCooldown(PlayerAbility.Tether))
                {
                    tetherAbility.TetherToggle();
                    if (currentMana <= 0 && tetherAbility.isTethered) 
                        tetherAbility.TetherToggle();
                        //Activate Portal UI
                }
                break;

            case PlayerAbility.Drain:
                if (!IsAbilityOnCooldown(PlayerAbility.Drain))
                {
                    drainAbility.enabled = !drainAbility.enabled;
                    cooldowns[PlayerAbility.Drain] = cooldownDuration;
                    //Activate Portal UI
                }
                break;
        }
    }

    bool IsAbilityOnCooldown(PlayerAbility ability)
    {
        return cooldowns.ContainsKey(ability);
    }
}