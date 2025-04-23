using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTether : MonoBehaviour
{
  public SpringJoint springJoint;
  public bool isTethered = false;
  private StateController controller;
  public float manaCost = 20f;
  public Rigidbody connectedBody;

    // Start is called before the first frame update
    void Start()
    {
        springJoint = GetComponent<SpringJoint>();
        controller = FindObjectOfType<StateController>();
        
    }

    public void Update()
    {
      connectedBody = GameObject.FindWithTag("Orb").GetComponent<Rigidbody>();
      springJoint.connectedBody = connectedBody; //Temporary solution for tether

      if (isTethered)
      {
        if (controller.currentMana <= 0)
        {
            TetherToggle();
        }
      }
    }

    public void TetherToggle()
    {
      if(isTethered == false)
      {
        springJoint.spring = 0f;
        springJoint.damper = 0f;
        isTethered = true;
        Debug.Log("Is not tethered");
      }
      else
      {
        springJoint.spring = 50f;
        springJoint.damper = 2f;
        isTethered = false;
        Debug.Log("IsTethered");

        controller.currentMana -= manaCost;
        controller.manaSlider.value = controller.currentMana;
      }
    }
}
