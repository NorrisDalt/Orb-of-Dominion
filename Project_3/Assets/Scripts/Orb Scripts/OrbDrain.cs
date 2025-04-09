using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbDrain : MonoBehaviour
{
    public PlayerMovement player;

    public float drainAmount = 5;
    
    private float hpStolen = 0;

    void OnTriggerStay(Collider col)
    {
        if(col.CompareTag("Enemy"))
        {
            Enemy enemy = col.GetComponent<Enemy>();
            DrainHealth(enemy, drainAmount);
        }
    }

    void DrainHealth(Enemy enemy, float hpDrained)
    {
        hpStolen = enemy.currentHealth - hpDrained;

        enemy.currentHealth -= hpDrained;

        player.pCurrentHealth += hpStolen;

        hpStolen = 0;
    }
}
