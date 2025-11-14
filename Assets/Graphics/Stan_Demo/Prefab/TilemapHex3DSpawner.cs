using UnityEngine;
using System.Collections.Generic;

public class Hex3DGenerator : MonoBehaviour
{
    [Header("References")]
    public GameObject[] hexTilePrefabs;    // 3D plane prefabs with hex textures

    [Header("Settings")]
    public int tileCount = 50;             // total tiles to place
    public int spread = 10;                // how far it can grow from the center
    public bool clearBeforeFill = true;

    [Header("Parent Settings")]
    public string parentName = "HexTileContainer";

    private List<GameObject> placedTiles = new List<GameObject>();
    private Transform parentContainer;

    [ContextMenu("Generate 3D Hex Grid")]
    public void GenerateHexGrid()
    {
        if (hexTilePrefabs == null || hexTilePrefabs.Length == 0)
        {
            Debug.LogWarning("No hex prefabs assigned!");
            return;
        }

        // Create/find parent container
        if (parentContainer == null)
        {
            GameObject parentObj = GameObject.Find(parentName);
            if (parentObj == null)
                parentObj = new GameObject(parentName);
            parentContainer = parentObj.transform;
        }
        else if (clearBeforeFill)
        {
            foreach (Transform child in parentContainer)
                DestroyImmediate(child.gameObject);
        }

        HashSet<Vector2Int> placed = new HashSet<Vector2Int>();
        List<Vector2Int> frontier = new List<Vector2Int>();

        Vector2Int startPos = Vector2Int.zero;
        frontier.Add(startPos);
        placed.Add(startPos);

        for (int i = 0; i < tileCount && frontier.Count > 0; i++)
        {
            int index = Random.Range(0, frontier.Count);
            Vector2Int current = frontier[index];
            frontier.RemoveAt(index);

            // Only instantiate if not already placed
            if (!placed.Contains(current))
                placed.Add(current);

            // Instantiate a random prefab
            GameObject prefab = hexTilePrefabs[Random.Range(0, hexTilePrefabs.Length)];
            Vector3 worldPos = HexCellToWorld(current);
            GameObject tileObj = Instantiate(prefab, worldPos, Quaternion.identity, parentContainer);
            placedTiles.Add(tileObj);

            // Explore neighbors
            foreach (Vector2Int dir in GetNeighborOffsets())
            {
                Vector2Int neighbor = current + dir;
                if (!placed.Contains(neighbor) &&
                    Mathf.Abs(neighbor.x) <= spread &&
                    Mathf.Abs(neighbor.y) <= spread)
                {
                    placed.Add(neighbor);
                    frontier.Add(neighbor);
                }
            }
        }

        Debug.Log($"Generated {placedTiles.Count} 3D hex tiles under '{parentName}'.");
    }

    // Convert hex cell to 3D world position (flat-top hex)
    private Vector3 HexCellToWorld(Vector2Int cell)
    {
        float prefabWidth = 1f;                     // set this to the actual width of your plane prefab
        float radius = prefabWidth / 2f;
        float h = Mathf.Sqrt(3f) / 2f * prefabWidth; // exact vertical spacing

        float worldX = 1.5f * radius * cell.x;  
        float worldZ = h * (cell.y + 0.5f * (cell.x % 2));

        return new Vector3(worldX, 0, worldZ);
    }

    // Neighbor offsets for flat-top hex grid
    private Vector2Int[] GetNeighborOffsets()
    {
        return new Vector2Int[]
        {
            new Vector2Int(+1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, +1),
            new Vector2Int(0, -1),
            new Vector2Int(+1, -1),
            new Vector2Int(-1, +1)
        };
    }
}
