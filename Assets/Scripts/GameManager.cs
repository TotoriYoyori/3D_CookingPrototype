using UnityEngine;

[DefaultExecutionOrder(-10)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TileManager tile_manager;
    [HideInInspector] public Tile current_tile; // WIP thing, needs to be assigned

    [Header("Game parameters")]
    public float uncommon_tile_chance;
    public float rare_tile_chance;

    private void Awake()
    {
        instance = this;
    }
}
