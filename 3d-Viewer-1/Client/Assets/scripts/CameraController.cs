using UnityEngine;

public abstract class CameraController : MonoBehaviour
{
    public abstract void Init(CameraController NextCameraController);


    public abstract void CameraRoutine(CameraScript CurrentController);

    public abstract void FocusOnTarget(GameObject target);


}
