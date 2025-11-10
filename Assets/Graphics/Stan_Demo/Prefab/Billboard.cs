using UnityEngine;

public class Billboard : MonoBehaviour
{
    public UnityEngine.Camera mainCamera;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = UnityEngine.Camera.main;
    }

    void LateUpdate()
    {
        // Make the sprite face the camera
        transform.forward = mainCamera.transform.forward;
    }
}
