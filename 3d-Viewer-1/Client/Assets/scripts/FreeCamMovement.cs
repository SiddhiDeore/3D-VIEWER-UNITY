using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamMovement : MonoBehaviour
{
    public float sensitivity;
    public float slowSpeed, normalSpeed, sprintSpeed;
    public float upDownSpeed; 
    float currentSpeed;
    bool isMovementEnabled = false;


    void Update()
    {
        Movement();

        if (Input.GetMouseButtonDown(1))
        {
            isMovementEnabled = !isMovementEnabled;
            Cursor.visible = !isMovementEnabled;
            Cursor.lockState = isMovementEnabled ? CursorLockMode.Locked : CursorLockMode.None;
        }

        if (isMovementEnabled)
        {
            
            Rotation();
        }
    }

    void Rotation()
    {
        Vector3 mouseInput = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        transform.Rotate(mouseInput * sensitivity * Time.deltaTime * 50);
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
    }

    void Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float upDownInput = Input.GetKey(KeyCode.Q) ? 1f : Input.GetKey(KeyCode.E) ? -1f : 0f;

        Vector3 input = new Vector3(horizontalInput, upDownInput, verticalInput);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            currentSpeed = slowSpeed;
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        transform.Translate(input * currentSpeed * Time.deltaTime);
    }
}
