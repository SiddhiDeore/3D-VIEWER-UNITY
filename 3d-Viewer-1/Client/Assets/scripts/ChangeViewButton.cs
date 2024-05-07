using UnityEngine;
using UnityEngine.UI;

public class CameraSwitchButton : MonoBehaviour
{
    public FreeCameraMovement freeCameraMovementScript;
    public CameraController orbitalCameraControllerScript;

    public void SwitchToFreeCamera()
    {
        // Enable free camera movement script
        freeCameraMovementScript.enabled = true;

        // Disable orbital camera controller script
        orbitalCameraControllerScript.enabled = false;
    }
}
