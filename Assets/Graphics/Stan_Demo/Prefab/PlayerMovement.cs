using UnityEngine;

public class PlayerMovementWithHoldHop : MonoBehaviour
{
    public float moveSpeed = 1f;       // Speed of the player

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D = left/right
        float moveZ = Input.GetAxis("Vertical");   // W/S = forward/back

        Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        if (moveX < -0.01f)
            spriteRenderer.flipX = false;  // Facing right
        else if (moveX > 0.01f)
            spriteRenderer.flipX = true;   // Facing left
    }
}
