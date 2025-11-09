using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    // List of all the tiles currently on the board
    [HideInInspector] public Dictionary<Vector2, Tile> tiles = new(); 
    
    // Draft markers are UI elements that appear when player is offered to explore a new tile
    [HideInInspector] public List<GameObject> draft_markers = new List<GameObject>();

    [Header("Prefabs and references")]
    [SerializeField] GameObject marker_prefab;
    [SerializeField] GameObject tile_highlight;
    [SerializeField] GameObject tiles_parent;
    public DraftingWindow draft;
    public SpriteRenderer preview;
    Vector3 next_tile_position = Vector3.zero;
    Quaternion next_tile_rotation = Quaternion.Euler(0,0,0);
    Vector2 next_tile_coords = Vector2.zero;
    [HideInInspector] public float tile_width = 0f;

    private void Awake()
    {
        InstantiateMarkers(); // spawning draft markers
    }

    void InstantiateMarkers()
    {
        for (int a = 0; a < 6; a++)
        {
            GameObject new_marker_prefab = Instantiate(marker_prefab, transform.position, Quaternion.Euler(90f, 0, 0), transform);
            draft_markers.Add(new_marker_prefab);
            new_marker_prefab.SetActive(false);
        }
    }

    public float FindTileDistance(Tile tile)
    {
        if (tile_width == 0) tile_width = tile.GetComponent<SpriteRenderer>().bounds.size.x; // ** fix this later (distance sometimes is a bit bigger than it should be
        float tile_distance = tile_width * Mathf.Cos(Mathf.PI / 6);

        return tile_distance;
    }

    public void ActivateDraftMarkers(Tile tile) // Acivating the markers that player can click to draft a new tile
    {
        for (int i = 0; i < 6; i++)
        {
            if (tile.paths[i] == Path.BLOCKED) continue; // there is no pathway
            //int connected_tile_index = (int)Mathf.Repeat(tile.entrance_point_id + i, 6);
            if (tile.GetConnectedTile(i) != null) continue; // there is another tile already

            // Spawning the marker near the correct tile
            draft_markers[i].SetActive(true);
            draft_markers[i].transform.position = tile.transform.position;

            // Setting markers rotation
            float marker_rotation = 180.0f - 60.0f * i - 60.0f * tile.entrance_point_id;
            draft_markers[i].transform.rotation = Quaternion.Euler(90f, 0, marker_rotation);
            
            Debug.Log("TileManager: -- NEW MARKER ("+i+") -- eulerY: " + draft_markers[i].transform.eulerAngles.y + ", eulerZ:" + draft_markers[i].transform.eulerAngles.z);


            // Positionning the marker correctly
            draft_markers[i].transform.position += draft_markers[i].transform.up * FindTileDistance(tile);

            // Setting marker coords
            NewMarkerCoords(draft_markers[i].GetComponent<DraftMarker>(), tile);
        }
    }

    // =========================================================
    // *** Moving player in between tiles
    // =========================================================
    public void HighlightTile(Tile tile_to_highlight, bool enable) // Highlight effect
    {
        if (tile_to_highlight == GameManager.instance.current_tile
            || tile_to_highlight == null
            || draft.gameObject.activeSelf == true) return; // no highlighting while drafting and you cant move to the same tile you're at

        tile_highlight.SetActive(enable);
        if (!enable) return;

        tile_highlight.transform.position = tile_to_highlight.transform.position;
    }

    public void MovePlayer(Tile tile_to_move)
    {
        GameManager.instance.player.MoveTo(tile_to_move);
        DeactivateMarkers();
        ActivateDraftMarkers(tile_to_move);
    }

    // =========================================================

    public void NewMarkerCoords(DraftMarker marker, Tile tile) // temporary solution for dynamically allocating x and y coords to a marker
    {
        //                          bottom               bottom left          top left            top               top right            bottom right
        //Vector2[] coord_mod = { new Vector2(0, -1), new Vector2(-1, 0), new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, -1), };
        Vector2 coord_mod = new Vector2(0,0);

        bool marker_x_equal = (Mathf.Abs(marker.transform.position.x - tile.transform.position.x) < 0.001f);
        bool marker_x_bigger = (marker.transform.position.x > tile.transform.position.x);
        bool marker_y_bigger = (marker.transform.position.z > tile.transform.position.z);

        if (marker_x_equal && marker_y_bigger) coord_mod = new Vector2(0, +1);
        else if (marker_x_equal && !marker_y_bigger) coord_mod = new Vector2(0, -1);
        else if (marker_x_bigger && marker_y_bigger) coord_mod = new Vector2(+1, 0);
        else if (marker_x_bigger && !marker_y_bigger) coord_mod = new Vector2(+1, -1);
        else if (!marker_x_bigger && marker_y_bigger) coord_mod = new Vector2(-1, +1);
        else if (!marker_x_bigger && !marker_y_bigger) coord_mod = new Vector2(-1, 0);

        marker.x = (int)(tile.x + coord_mod.x);
        marker.y = (int)(tile.y + coord_mod.y);
    }

    public Tile GetTile(int x, int y) // Easier method for accesing tiles
    {
        return tiles[new Vector2(x, y)];
    }

    public void NewDraft(Vector3 tile_position, Quaternion tile_rotation, Vector2 tile_coordinates) // Activating the draft choice UI window
    {
        next_tile_position = tile_position;
        next_tile_rotation = tile_rotation;
        next_tile_coords = tile_coordinates;

        // Activating Draft UI
        draft.gameObject.SetActive(true);
        draft.ActivateDraft(FetchTileOptions(GameManager.instance.current_tile), tile_position);

        // Activating preview tile
        preview.transform.position = tile_position;
        preview.transform.rotation = tile_rotation;

        //DeactivateMarkers();
    }

    Tile[] FetchTileOptions(Tile tile_from)
    {
        // make logic for the game to know which tiles to ask for
        TileList all_tile_list= GetComponent<TileList>();
        Tile[] tiles_to_return = new Tile[3];

        // First tile is any tile of the same biome
        tiles_to_return[0] = all_tile_list.GetDraftTile(tile_from.biome, GetRandomRarity());

        // Second tile cannot dublicate first one
        do
        {
            tiles_to_return[1] = all_tile_list.GetDraftTile(tile_from.biome, GetRandomRarity());
        } while (tiles_to_return[1] == tiles_to_return[0]);

        // Third tile cannot be of the same biome as first two
        do
        {
            tiles_to_return[2] = all_tile_list.GetDraftTile(GetRandomBiome(), GetRandomRarity());
        } while (tiles_to_return[2].biome == tiles_to_return[1].biome);
        

        return tiles_to_return;
    }

    Rarity GetRandomRarity()
    {
        float random_n = Random.value;

        if (random_n < GameManager.instance.rare_tile_chance) return Rarity.RARE;
        else if (random_n < GameManager.instance.uncommon_tile_chance) return Rarity.UNCOMMON;

        return Rarity.BASIC;
    }

    Biome GetRandomBiome()
    {
        int random_biome_id = Random.Range(0, System.Enum.GetValues(typeof(Biome)).Length);

        return (Biome)random_biome_id;
    }

    public void PlaceTile(Tile tile) // Spawning the tile that the player chooses
    {
        Tile new_tile = Instantiate(tile, next_tile_position, next_tile_rotation, tiles_parent.transform);
        new_tile.InitTile((int)next_tile_coords.x, (int)next_tile_coords.y); // setting tile coordinates
    }
    public void DeactivateMarkers()
    {
        foreach (GameObject marker in draft_markers)
        {
            marker.GetComponent<DraftMarker>().Deactivate();
        }
    }
}
