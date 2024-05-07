using UnityEngine;
using System;

public class TouchControlCamera : CameraController
{
    [Header("Target Settings")]
    public Transform targetObject;

    [Header("Control Settings")]
    public float rotationSpeed = 0.5f;
    public float zoomSpeed = 0.1f;
    public float minY = -10f;
    public float maxY = 10f;
    public bool touchControlsEnabled = true;
    public float movementSpeed = 10;
    // Adjust this value as needed to set how close the camera starts near the target
    public float initialDistanceToTarget = 15.0f;

    [Header("Zoom Limits")]
    private float currentZoom;
    private float minZoom = 5f;
    private float maxZoom = 50f;

    [Header("State")]
    private Vector3 pivot;
    private Vector3 pivot2;
    private Vector3 initialOffset;
    private float currentYaw = 0f;
    private float currentPitch = 0f;
    private bool isMovingUp = false, isMovingDown = false, isMovingLeft = false, isMovingRight = false;
    private bool isMovementButtonPressed = false;
    private bool controlsInitialized = false;
    public bool stopButtonPressed = false;
    private bool isTouchCameraActive = true;

    [Header("Delegates")]
    private Action touchRotationDelegate;
    private Action pinchZoomDelegate;
    private Action cameraTransformDelegate;

    [Header("Camera Control")]
    public CameraController NextCameraController;
    public CameraScript cameraScript;
    public UI Uimanager;

    public override void Init(CameraController freeCameraMovementScript)
    {
        NextCameraController = freeCameraMovementScript;
        Uimanager.SetUI(true);
        // Setting the pivot to the targetObject's position
        pivot = targetObject.transform.position;

        // Calculating a direction vector from the camera to the pivot (target object)
        Vector3 directionToTarget = (pivot - transform.position).normalized;

        // Setting the camera's new position close to the target object, maintaining the calculated direction
        transform.position = pivot - (directionToTarget * initialDistanceToTarget);

        // Recalculating the initialOffset based on the new position
        initialOffset = transform.position - pivot;

        // Other initial setup remains the same
        pivot2 = pivot;
        //initialOffset = transform.position - pivot;
        currentZoom = initialOffset.magnitude;
        EnableTouchCamera();

    }

    public override void FocusOnTarget(GameObject target)
    {
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position + new Vector3(0, 3, -5); // Example offset for a touch-controlled camera
            Camera.main.transform.position = targetPosition;
            Camera.main.transform.LookAt(target.transform);
        }
    }

    public override void CameraRoutine(CameraScript cameraScript)
    {
        if (isTouchCameraActive) // Check if touch camera is active
        {
            if (!stopButtonPressed)
            {
                // If the stop button has not been pressed, continue with the usual control checks
                touchRotationDelegate?.Invoke();
                pinchZoomDelegate?.Invoke();
                cameraTransformDelegate?.Invoke();
                EnableControls();
            }
            else
            {
                DisableControls();
            }
        }
    }

    void DisableTouchCamera()
    {
        isTouchCameraActive = false;
        // Additional code to stop any ongoing touch camera controls if needed
    }

    // Method to enable touch camera controls
    void EnableTouchCamera()
    {
        isTouchCameraActive = true;
    }

    public void DisableControls()
    {
        // Unbind all methods from delegates
        StopAllMovement(cameraScript);
    }

    // Add a method to handle the re-enabling of controls, resetting the stopButtonPressed flag
    public void EnableControls()
    {
        // Bind methods to delegates
        touchRotationDelegate = HandleTouchRotation;
        pinchZoomDelegate = HandlePinchZoom;
        cameraTransformDelegate = ApplyCameraTransform;
        controlsInitialized = true;  
    }

    private void StopAllMovement(CameraScript cameraScript)
    {
        // Nullify the delegates to ensure no control methods are called
        touchRotationDelegate = null;
        pinchZoomDelegate = null;
        cameraTransformDelegate = null;
        controlsInitialized = false;
        Uimanager.SetUI(false);
        FreeCameraMovement freeCameraMovementScript = NextCameraController as FreeCameraMovement;
        cameraScript.CurrentController = NextCameraController;
        cameraScript.CurrentController.Init(this);        
    }

    private void HandleTouchRotation()
    {
        if (isMovementButtonPressed || Input.touchCount != 1) return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Moved)
        {
            currentYaw += touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
            currentPitch -= touch.deltaPosition.y * rotationSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minY, maxY);
        }
    }

    private void HandlePinchZoom()
    {
        if (isMovementButtonPressed || Input.touchCount != 2) return;

        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            currentZoom += deltaMagnitudeDiff * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
    }

    private void ApplyCameraTransform()
    {
        UpdateMovement();

        // Assuming targetObject has a Renderer or Collider to define its bounds
        Bounds bounds = new Bounds();
        if (targetObject.GetComponent<Renderer>() != null)
        {
            bounds = targetObject.GetComponent<Renderer>().bounds;
        }
        else if (targetObject.GetComponent<Collider>() != null)
        {
            bounds = targetObject.GetComponent<Collider>().bounds;
        }
        else
        {
            // If no Renderer or Collider is found, we can't determine bounds.
            Debug.LogWarning("Target object has no Renderer or Collider to determine bounds.");
            return;
        }

        // Calculate maxDistance based on the bounds of the target object
        // You can adjust this calculation to fit your specific needs
        Vector3 boundsSize = bounds.size;
        float maxDistanceX = boundsSize.x / 2;
        float maxDistanceY = boundsSize.y / 2;
        float maxDistanceZ = boundsSize.z / 2;

        // Calculate the displacement from pivot to pivot2 and restrict movement based on maxDistance
        Vector3 displacement = pivot2 - pivot;

        displacement.x = Mathf.Clamp(displacement.x, -maxDistanceX, maxDistanceX);
        displacement.y = Mathf.Clamp(displacement.y, -maxDistanceY, maxDistanceY);
        displacement.z = Mathf.Clamp(displacement.z, -maxDistanceZ, maxDistanceZ);

        // Adjust pivot2 based on the clamped displacement
        pivot2 = pivot + displacement;

        // Apply camera transformation using the adjusted pivot2
        Vector3 direction = new Vector3(0, 0, -currentZoom);
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        transform.position = pivot2 + rotation * direction;
        transform.LookAt(pivot2);
    }

    private void UpdateMovement()
    {
        if (isMovingUp)
            pivot2 += transform.up * movementSpeed * Time.deltaTime;
        if (isMovingDown)
            pivot2 -= transform.up * movementSpeed * Time.deltaTime;
        if (isMovingLeft)
            pivot2 -= transform.right * movementSpeed * Time.deltaTime;
        if (isMovingRight)
            pivot2 += transform.right * movementSpeed * Time.deltaTime;
    }

    private void CheckAllButtonsReleased()
    {
        if (!isMovingUp && !isMovingDown && !isMovingLeft && !isMovingRight)
        {
            isMovementButtonPressed = false;
        }
    }

    public void MoveUp() { isMovingUp = true; isMovementButtonPressed = true; }
    public void MoveDown() { isMovingDown = true; isMovementButtonPressed = true; }
    public void MoveLeft() { isMovingLeft = true; isMovementButtonPressed = true; }
    public void MoveRight() { isMovingRight = true; isMovementButtonPressed = true; }

    public void StopUp() { isMovingUp = false; CheckAllButtonsReleased(); }
    public void StopDown() { isMovingDown = false; CheckAllButtonsReleased(); }
    public void StopLeft() { isMovingLeft = false; CheckAllButtonsReleased(); }
    public void StopRight() { isMovingRight = false; CheckAllButtonsReleased(); }



}

