using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public CharacterController Controller;
    public float speed;
    public float ObjectiveCollected = 0;

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (direction.magnitude >= 0.1f)
            {
                Controller.Move(direction * speed * Time.deltaTime);
            }

        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objective"))
        {
            ObjectiveCollected =+1;
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Objective"))
        {
            ObjectiveCollected = +1;
        }
    }
}
