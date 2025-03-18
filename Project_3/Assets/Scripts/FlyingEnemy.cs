using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public GameObject flyingType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = flyingType.transform.position;

       // newPosition.y = Vector3.MoveTowards()

        flyingType.transform.position = newPosition;
    }
}
