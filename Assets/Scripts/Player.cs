using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.player = this;
    }
    public void MoveTo(Tile tile_to_move)
    {
        // fast movement logic, make better from above later!!
        transform.position = tile_to_move.transform.position;
        transform.Translate(0, -0.4f, -0.5f);

        // Settijng new current tile
        GameManager.instance.current_tile.SetAsCurrentTile(false);
        tile_to_move.SetAsCurrentTile(true);
    }
}
