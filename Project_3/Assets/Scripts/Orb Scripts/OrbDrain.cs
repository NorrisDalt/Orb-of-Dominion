using System.Collections;
using UnityEngine;

public class OrbDrain : MonoBehaviour
{
    [Header("Drain Settings")]
    public float drainAmount = 5f;
    public float drainInterval = 1.5f;
    
    [Header("References")]
    public PlayerMovement player;
    
    private bool isDraining = false;
    private Enemy currentEnemy;
    public StateController controller;
    public float manaCost = 9f;

    void OnTriggerStay(Collider col)
    {
        if(col.CompareTag("Enemy") && !isDraining)
        {
            currentEnemy = col.GetComponent<Enemy>();
            if(currentEnemy != null && currentEnemy.currentHealth > 0)
            {
                StartCoroutine(DrainHealthRoutine());
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.CompareTag("Enemy") && col.GetComponent<Enemy>() == currentEnemy)
        {
            StopDraining();
        }
    }

    IEnumerator DrainHealthRoutine()
    {
        isDraining = true;
        
        while(currentEnemy != null && currentEnemy.currentHealth > 0 && player.pCurrentHealth < player.pMaxHealth && controller.currentMana > 0)
        {
            // Calculate how much we can drain without exceeding player's max health
            float possibleDrain = player.pMaxHealth - player.pCurrentHealth;
            float actualDrain = Mathf.Min(drainAmount, currentEnemy.currentHealth, possibleDrain);

            //Calculate mana
            controller.currentMana -= manaCost;
            controller.manaSlider.value = controller.currentMana;
            
            if(actualDrain <= 0) break; // Stop if we can't drain anything
            
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