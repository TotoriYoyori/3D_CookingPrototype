using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    public UnityEngine.Camera mainCamera;

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // Make the sprite face the camera
        transform.forward = mainCamera.transform.forward;
    }
}
