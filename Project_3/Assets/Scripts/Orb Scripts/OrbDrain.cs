using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OrbDrain : MonoBehaviour
{
    [Header("Drain Settings")]
    public float drainAmount = 5f;
    public float drainInterval = 1.5f;
    public float manaCost = 9f;
    
    [Header("References")]
    public PlayerMovement player;  // Now properly declared at class level
    public StateController controller;  // Now properly declared at class level
    
    public bool isDraining = false;
    private Enemy currentEnemy;


    void Start()
    {
        InitializeReferences();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reinitialize references when a new scene loads
        InitializeReferences();
    }

    void InitializeReferences()
    {
        player = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();
        controller = FindObjectOfType<StateController>();
        
        if (player == null) 
            Debug.LogWarning("Player reference not found in OrbDrain");
        if (controller == null) 
            Debug.LogWarning("StateController reference not found in OrbDrain");
    }

    void Update()
    {
        controller = FindObjectOfType<StateController>();
        // Only run drain logic if enabled
        if (!enabled) return;
        
        // Automatic disable if no mana
        if (controller != null && controller.currentMana <= 0)
        {
            enabled = false;
            return;
        }
        InitializeReferences();
    }

    public void CheckDrainTrigger(Collider col)
    {
        if (enabled && col.CompareTag("Enemy") && !isDraining)
        {
            currentEnemy = col.GetComponent<Enemy>();
            if (currentEnemy != null && currentEnemy.currentHealth > 0)
            {
                StartCoroutine(DrainHealthRoutine());
            }
        }
    }

    IEnumerator DrainHealthRoutine()
    {
        isDraining = true;
        
        while(enabled && currentEnemy != null && 
              currentEnemy.currentHealth > 0 && 
              player != null && player.pCurrentHealth < player.pMaxHealth && 
              controller != null && controller.currentMana > 0)
        {
            float possibleDrain = player.pMaxHealth - player.pCurrentHealth;
            float actualDrain = Mathf.Min(drainAmount, currentEnemy.currentHealth, possibleDrain);
            
            if(actualDrain <= 0) break;
            
            // Apply mana cost
            controller.currentMana -= manaCost;
            controller.manaSlider.value = controller.currentMana;
            
            // Transfer health
            currentEnemy.currentHealth -= actualDrain;
            player.pCurrentHealth += actualDrain;

            yield return new WaitForSeconds(drainInterval);
        }
        
        StopDraining();
    }

    void StopDraining()
    {
        StopAllCoroutines();
        isDraining = false;
        currentEnemy = null;
    }

    void OnDisable()
    {
        StopDraining();
    }
}