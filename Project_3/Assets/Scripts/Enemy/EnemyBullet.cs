using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
   void OnCollisionEnter(Collision col)
   {
    if(col.gameObject.CompareTag("Player")){
        Destroy(this.gameObject);
    }
   }
    
}
