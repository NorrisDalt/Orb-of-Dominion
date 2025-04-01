using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTether : MonoBehaviour
{
  public SpringJoint springJoint;

    // Start is called before the first frame update
    void Start()
    {
        springJoint = GetComponent<SpringJoint>();
    }

    public void FixedUpdate()
    {
      TetherToggle();
    }

    void TetherToggle()
    {
      if(Input.GetMouseButtonDown(1))
      {
        springJoint.spring = 0f;
        springJoint.damper = 0f;
      }
    }
}
