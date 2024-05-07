using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OrbitalCamera : MonoBehaviour
{
    public Transform targetObject;
    public float zoomSpeed = 4f;
    private float minZoom; // Now private and not set in advance
    private float maxZoom; // Now private and not set in advance
    public float rotationSpeed = 100f;
    public float sensitivity = 10f;
    public float minY = -20f;
    public float maxY = 80f;

    private float currentZoom;
    private Vector3 initialOffset;
    private float currentYaw = 0f;
    private float currentPitch = 0f;

    void Start()
    {
        initialOffset = transform.position - targetObject.position;
        AdjustZoomBasedOnTargetSize();
        currentZoom = Mathf.Clamp(initialOffset.magnitude, minZoom, maxZoom);
    }

    void Update()
    {
        // Zoom in and out with mouse scroll wheel
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // Rotate camera around the object with mouse
        if (Input.GetMouseButton(1)) // Right mouse button held down
        {
            currentYaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentPitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, -89f, 89f); // Prevent flipping
        }

        if(Input.GetMouseButton(1)) // Right mouse button held down
        {
            currentYaw += Input.GetAxis("Mouse X") * sensitivity;
            currentPitch -= Input.GetAxis("Mouse Y") * sensitivity;
            currentPitch = Mathf.Clamp(currentPitch, minY, maxY);
        }

        // Optionally, rotate camera with keyboard arrows
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            currentYaw -= sensitivity * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            currentYaw += sensitivity * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            currentPitch += sensitivity * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minY, maxY);
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            currentPitch -= sensitivity * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minY, maxY);
        }
    }

    void LateUpdate()
    {
        // Apply zoom directly towards or away from the target
        Vector3 direction = (transform.position - targetObject.position).normalized;
        transform.position = targetObject.position + direction * currentZoom;

        // Apply rotation
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        transform.position = targetObject.position + transform.rotation * initialOffset.normalized * currentZoom;

        // Ensure camera always faces the target
        transform.LookAt(targetObject.position);
    }

    void AdjustZoomBasedOnTargetSize()
    {
        // Dynamically calculate minZoom and maxZoom based on the object's dimensions
        MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
        if (meshFilter)
        {
            Bounds bounds = meshFilter.mesh.bounds;
            float maxSize = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            // Adjust minZoom and maxZoom dynamically
            minZoom = maxSize; // Prevents camera from going too close
            maxZoom = maxSize * 10f; // Allows camera to zoom out and see the object fully
        }
        else
        {
            Debug.LogWarning("Target object does not have a MeshFilter component. Using fallback zoom limits.");
            // Fallback zoom limits if no MeshFilter is found
            minZoom = 1f;
            maxZoom = 10f;
        }
    }
}
