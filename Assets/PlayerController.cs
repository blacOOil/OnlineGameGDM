using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    
    public CharacterController Controller;
    public float speed;
    public float runSpeed;
    public float ObjectiveCollected = 0;
    public float mouseSensitivity = 100f;

    public GameObject playerCam;

    Vector3 moveDirection = Vector3.zero;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    float rotationX = 0;
    public bool canMove = true;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCam.SetActive(false);
        runSpeed = speed + 10;
        canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            playerCam.SetActive(true);
        }
        if (canMove == true) { 
        PlayerMovementing();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            canMove = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void PlayerMovementing()
    {
        if (IsOwnedByServer || IsClient)
        {
            // Handle rotation
            rotationX += -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime, 0);

            // Handle movement
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection).normalized;

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = (isRunning ? runSpeed : speed) * Input.GetAxis("Vertical") * Time.deltaTime;
            float curSpeedY = (isRunning ? runSpeed : speed) * Input.GetAxis("Horizontal") * Time.deltaTime;

       
            Vector3 velocity = moveDirection * speed ;
            velocity.y = rb.velocity.y; // Maintain current vertical velocity

            rb.velocity = velocity;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Objective"))
        {
            ObjectiveCollected += 1;
            Debug.Log("Objective collected! Total: " + ObjectiveCollected);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Objective"))
        {
            ObjectiveCollected += 1;
            Debug.Log("Objective collision! Total: " + ObjectiveCollected);
        }
    }

}