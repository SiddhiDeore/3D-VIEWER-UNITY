using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchCamera : MonoBehaviour
{
    public FixedJoystick moveJoystick;
    public float moveSpeed = 5f;
    public float upDownSpeed = 2f;
    public float rotationSpeed = 10f;
    private CharacterController controller;
    private Vector2 touchStartPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();
        Rotation();
    }

    void Movement()
    {
        
        Vector3 moveDirection = transform.right * moveJoystick.Horizontal + transform.forward * moveJoystick.Vertical;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    void Rotation()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); 

            if (touch.position.x > Screen.width / 2)
            {
                if (touch.phase == TouchPhase.Moved)
                {
                    float rotationInputX = touch.deltaPosition.x;
                    float rotationInputY = touch.deltaPosition.y;
                    transform.Rotate(Vector3.up, rotationInputX * rotationSpeed * Time.deltaTime);
                    transform.Rotate(Vector3.left, rotationInputY * upDownSpeed * Time.deltaTime);
                    Vector3 eulerRotation = transform.rotation.eulerAngles;
                    transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0);
                }
            }
        }
    }
}
