using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 10f;  // Speed of camera movement
    public float minY = 5f;        // Min camera height
    public float maxY = 20f;       // Max camera height

    void Update()
    {
        float moveX = 0f;
        float moveZ = 0f;

        // JKLI input
        if (Input.GetKey(KeyCode.J)) moveX = -1f; // left
        if (Input.GetKey(KeyCode.L)) moveX = 1f;  // right
        if (Input.GetKey(KeyCode.I)) moveZ = 1f;  // up
        if (Input.GetKey(KeyCode.K)) moveZ = -1f; // down

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}
