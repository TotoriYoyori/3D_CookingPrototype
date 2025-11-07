using Unity.VisualScripting;
using UnityEngine;

public enum Rarity
{
    BASIC,
    UNCOMMON,
    RARE,
    UNIQUE
}

public enum Biome
{
    GRASSLANDS,
    WETLANDS,
    COASTLANDS,
    UNDERLANDS,
    FORAGELANDS
}

public enum Path
{
    OPEN, 
    BLOCKED,
    WATER
}
public class Tile : MonoBehaviour
{
    [Header("Tile properties")] // properties of each individual tiles
    public Path[] paths;
    public Rarity rarity;
    public Biome biome;
    public string t_name;
    public string t_description;
    [HideInInspector] public int x;
    [HideInInspector] public int y;

    [Header("Tile components")] // Tile gameobject components
    public GameObject[] sides;
    [SerializeField] GameObject[] borders;
    [SerializeField] GameObject border_obj;
    public SpriteRenderer sprite; 

    [Header("Object refs")] // references to other gameObjects
    public Tile[] connected_tiles = new Tile[6];
    TileManager tile_manager;
    [HideInInspector] public int entrance_point_id = 0;

    [Header("misc")]
    [SerializeField] bool initialize_on_start;

    private void Start()
    {
        if (initialize_on_start) InitTile(0, 0); // only one can be initialized on start
    }

    // =====================================
    // *** Tile Initialization ***
    // =====================================

    public void InitTile(int coord_x, int coord_y)
    {
        x = coord_x;
        y = coord_y;

        tile_manager = GameManager.instance.tile_manager;
        tile_manager.tiles.Add(new Vector2(x, y), this);

        // Temporary setting current tile here
        GameManager.instance.current_tile = this;

        // Check if there are any tiles around that this tile could connect to
        CheckTilesAround();

        // Set entrance point id (0- bottom, 1- left bottom and so on)
        SetEntrancePoint();

        // After initializing draft choices are initiated right away
        tile_manager.ActivateDraftMarkers(this);

        Debug.Log("Tile: New Tile placed with coords: " + x + ", " + y);
    }

    void SetEntrancePoint()
    {
        entrance_point_id = (6 - (int)Mathf.Repeat(transform.eulerAngles.z, 360f) / 60) % 6;
    }

    void CheckTilesAround() // checking for any neightbor tiles
    {
        Vector2[] coords_around = GetCoordsAround();
        for (int a = 0; a < coords_around.Length; a++)
        {
            if (tile_manager.tiles.ContainsKey(coords_around[a]))
            {
                Tile tile_to_connect = tile_manager.tiles[coords_around[a]];
                ConnectTile(tile_to_connect, a);

                Debug.Log("Tile: " + tile_to_connect.t_name + " is connected to " + t_name);
            }
        }
    }

    void ConnectTile(Tile tile_to_connect, int index) // Making sure both tiles recognize each other as neighbor tiles
    {
        connected_tiles[index] = tile_to_connect;

        int other_tile_index = (index + 3) % 6;
        tile_to_connect.connected_tiles[other_tile_index] = this;
    }

    Vector2[] GetCoordsAround() // Giving a list of coordinates for potential tiles that could be around a tile with named coords
    {
        Vector2[] to_return = new Vector2[6];

        to_return[0] = new Vector2(x, y - 1);
        to_return[1] = new Vector2(x - 1, y);
        to_return[2] = new Vector2(x -1, y + 1);
        to_return[3] = new Vector2(x, y + 1);
        to_return[4] = new Vector2(x + 1, y);
        to_return[5] = new Vector2(x + 1, y -1);

        return to_return;
    }
    
    // =====================================
    // *** Tile Functionality ***
    // =====================================
    private void Update()
    {
        CheckBordersInput();
    }

    void CheckBordersInput() // visually shows tile borders when "B" is held
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            EnableVisibleBorders(true);
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            EnableVisibleBorders(false);
        }
    }

    void EnableVisibleBorders(bool is_enabled)
    {
       for (int a = 0; a < borders.Length; a++)
       {
            if (paths[a] == Path.BLOCKED) borders[a].SetActive(is_enabled);
       }
    }

    public Tile GetConnectedTile(int side_index) // gives the connected tile in relation to provivded side index (0- bottom, 1- bottom left and so on)
    {
        int connected_tile_index = (int)Mathf.Repeat(entrance_point_id + side_index, 6);
        return connected_tiles[connected_tile_index];
    }
}
