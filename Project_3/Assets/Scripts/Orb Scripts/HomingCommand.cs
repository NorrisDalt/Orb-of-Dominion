using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingCommand : MonoBehaviour
{
    Projectile projectile;
    public GameObject orb;
    public float hommingSpeed = 10f;
    public float offsett = 0.5f;

    private Vector3 tempPosition; 

    // Start is called before the first frame update
    void Start()
    {
        tempPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        HommingAttack();
    }
    
    public void HommingAttack()
    {
        Vector3 newOffSet = tempPosition + Vector3.up * offsett;
        orb.transform.position = Vector3.MoveTowards(projectile.hitPosition, newOffSet, hommingSpeed * Time.deltaTime);
    }
}
