using UnityEngine;

public class BillboardSprite : MonoBehaviour
{
    [SerializeField] GameObject sprite_to_billboard;

    private void Start()
    {
        // by default it will rotate teh object this script is attached to
        if (sprite_to_billboard == null) sprite_to_billboard = this.gameObject;
    }

    void LateUpdate()
    {
        // Get the camera
        Camera mainCamera = GameManager.instance.main_camera;

        // Make the sprite face the camera
        sprite_to_billboard.transform.forward = mainCamera.transform.forward;
    }
}
