using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PhoneScript : NetworkBehaviour
{
    public Animator phone;
    bool isphoneUp;
    void Start()
    {
        isphoneUp = false;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if(isphoneUp == false)
            {
                isphoneUp = true;
            }
            else
            {
                isphoneUp = false;
            }
        }
        PhoneController();
    }
    public void PhoneController()
    {
        if (isphoneUp == true)
        {
            phone.SetInteger("IsPhoneUp", 1);
        }
        else
        {
            phone.SetInteger("IsPhoneUp", 0);
        }
    }
}
