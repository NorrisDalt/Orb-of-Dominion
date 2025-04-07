using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float bulletDamge = 10f;
    void OnCollisionEnter(Collision col)
   {
        if(col.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = col.gameObject.GetComponent<PlayerMovement>();
            player.TakeDamage(bulletDamge);
            Destroy(this.gameObject);
        }
   }
    
}
