using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.player = this;
    }
    public void MoveTo(Tile tile_to_move)
    {
        /*
        Vector3 movement_vector = new Vector2();
        movement_vector.x = tile_to_move.x - transform.position.x;
        movement_vector.y = tile_to_move.y - transform.position.y;

        transform.position += movement_vector;*/

        // fast movement logic, make better from above later!!
        transform.position = tile_to_move.transform.position;
        transform.Translate(0, -0.4f, -0.5f);

        GameManager.instance.current_tile = tile_to_move;
    }
}
