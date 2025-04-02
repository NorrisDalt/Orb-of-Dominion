using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTether : MonoBehaviour
{
  public SpringJoint springJoint;
  private bool isTethered = false;

    // Start is called before the first frame update
    void Start()
    {
        springJoint = GetComponent<SpringJoint>();
    }

    public void FixedUpdate()
    {
      //TetherToggle();
    }

    public void TetherToggle()
    {
      if(isTethered == true)
      {
        springJoint.spring = 0f;
        springJoint.damper = 0f;
        isTethered = false;
        Debug.Log("Is not tethered");
      }
      else
      {
        springJoint.spring = 200f;
        springJoint.damper = 2f;
        isTethered = true;
        Debug.Log("IsTethered");
      }
    }
}
