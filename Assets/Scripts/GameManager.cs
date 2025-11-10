using UnityEngine;

[DefaultExecutionOrder(-10)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TileManager tile_manager;
    public InventoryManager inventory_manager;
    public Player player;
    public CameraBehavior main_camera;
    [HideInInspector] public Tile current_tile; // WIP thing, needs to be assigned

    [Header("Game parameters")]
    public float uncommon_tile_chance;
    public float rare_tile_chance;

    private void Awake()
    {
        instance = this;
    }

    // Checks if player can move around tiles and draft new tiles
    public bool ExplorationEnabled()
    {
        if (tile_manager.draft.gameObject.activeSelf) return false; // exploration locked while drafting

        return true;
    }
}
