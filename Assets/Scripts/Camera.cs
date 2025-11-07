using Unity.VisualScripting;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [HideInInspector]
    public Vector3 to_follow;
    [SerializeField] float speed;
    [SerializeField] float y_offset;
    [SerializeField] float z_offset;
    float camera_left_border, camera_right_border, camera_top_border, camera_bottom_border, camera_height, camera_width;

    private void Start()
    {
        GameManager.instance.main_camera = this;
    }

    void FixedUpdate()
    {
        Follow();
        //CameraBorders();
    }
    void Follow()
    {
        to_follow = GameManager.instance.player.transform.position;
        if (to_follow == null) return;
        transform.position = Vector3.Lerp(transform.position, to_follow, speed);
        transform.position += new Vector3(0, y_offset, z_offset); // Fix this!!!
    }
    /*
    void CameraBorders()
    {
        camera_height = GetComponent<Camera>().orthographicSize;
        camera_width = GetComponent<Camera>().aspect * camera_height;

        camera_left_border = transform.position.x - camera_width / 2;
        camera_bottom_border = transform.position.y - camera_height / 2;
        camera_right_border = transform.position.x + camera_width / 2;
        camera_top_border = transform.position.y + camera_height / 2;

        if (camera_left_border < Game.level.left_level_border) transform.position =
                new Vector3(Game.level.left_level_border + camera_width / 2, transform.position.y, transform.position.z);
        if (camera_bottom_border < Game.level.bottom_level_border) transform.position =
                new Vector3(transform.position.x, Game.level.bottom_level_border + camera_height / 2, transform.position.z);
        if (camera_right_border > Game.level.right_level_border) transform.position =
                new Vector3(Game.level.right_level_border - camera_width / 2, transform.position.y, transform.position.z);
        if (camera_top_border > Game.level.top_level_border) transform.position =
                new Vector3(transform.position.x, Game.level.top_level_border - camera_height / 2, transform.position.z);
    }*/
}
