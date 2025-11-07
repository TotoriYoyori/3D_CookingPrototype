using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapRandomShapeFiller : MonoBehaviour
{
    [Header("References")]
    public Tilemap tilemap;
    public TileBase[] tileOptions;

    [Header("Settings")]
    public int tileCount = 50; // total tiles to place
    public int spread = 10;    // how far it can grow from the center
    public bool clearBeforeFill = true;

    [ContextMenu("Generate Random Shape")]
    public void GenerateRandomShape()
    {
        if (tilemap == null || tileOptions == null || tileOptions.Length == 0)
        {
            Debug.LogWarning("Missing Tilemap or Tile Options!");
            return;
        }

        if (clearBeforeFill)
            tilemap.ClearAllTiles();

        HashSet<Vector3Int> placed = new HashSet<Vector3Int>();
        List<Vector3Int> frontier = new List<Vector3Int>();

        // Start from the center
        Vector3Int startPos = Vector3Int.zero;
        frontier.Add(startPos);
        placed.Add(startPos);

        for (int i = 0; i < tileCount && frontier.Count > 0; i++)
        {
            // Pick a random frontier tile
            int index = Random.Range(0, frontier.Count);
            Vector3Int current = frontier[index];
            frontier.RemoveAt(index);

            // Place a random tile
            TileBase randomTile = tileOptions[Random.Range(0, tileOptions.Length)];
            tilemap.SetTile(current, randomTile);

            // Explore neighboring positions (works for hex or square grids)
            foreach (Vector3Int dir in GetNeighborOffsets())
            {
                Vector3Int neighbor = current + dir;
                if (!placed.Contains(neighbor) &&
                    Mathf.Abs(neighbor.x) <= spread &&
                    Mathf.Abs(neighbor.y) <= spread)
                {
                    placed.Add(neighbor);
                    frontier.Add(neighbor);
                }
            }
        }

        Debug.Log($"Generated random shape with {placed.Count} tiles.");
    }

    // Offsets for hex or square grids
    private Vector3Int[] GetNeighborOffsets()
    {
        // For flat-top hex grids
        return new Vector3Int[]
        {
            new Vector3Int(+1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, +1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(+1, -1, 0),
            new Vector3Int(-1, +1, 0)
        };
    }
}
