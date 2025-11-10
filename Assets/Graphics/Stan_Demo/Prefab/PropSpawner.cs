using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class PropSpawner : MonoBehaviour
{
    [Header("Prop Settings")]
    public GameObject propPrefab;         // prefab with SpriteRenderer + Billboard
    public Sprite[] propSprites;          // all your 2D sprites
    public int maxPropsPerTile = 3;       // max props per tile
    public float spreadRadius = 0.3f;     // radius to spread props on XZ plane
    public float minDistance = 0.1f;      // minimum distance between props

    [Header("Tilemap Reference")]
    public Tilemap tilemap;               // reference to your tilemap

    [Header("Parenting")]
    public Transform parent;              // optional parent for organization

    [ContextMenu("Spawn Props")]
    public void SpawnProps()
    {
        if (propPrefab == null || propSprites.Length == 0 || tilemap == null)
        {
            Debug.LogWarning("Missing propPrefab, propSprites, or tilemap!");
            return;
        }

        // --- Clear previously spawned props ---
        if (parent != null)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }

        // --- Collect occupied tiles ---
        List<Vector3Int> occupiedTiles = new List<Vector3Int>();
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                occupiedTiles.Add(pos);
            }
        }

        if (occupiedTiles.Count == 0)
        {
            Debug.LogWarning("No tiles found on tilemap to spawn props!");
            return;
        }

        // --- Spawn props ---
        foreach (Vector3Int tilePos in occupiedTiles)
        {
            int propsOnTile = Random.Range(0, maxPropsPerTile + 1);
            Vector3 centerPos = tilemap.GetCellCenterWorld(tilePos);

            List<Vector3> spawnedPositions = new List<Vector3>();

            for (int i = 0; i < propsOnTile; i++)
            {
                Vector3 spawnPos;
                int attempts = 0;
                const int maxAttempts = 20;

                // Try to find a position that respects minDistance
                do
                {
                    float angle = Random.Range(0f, Mathf.PI * 2f);
                    float radius = Random.Range(0f, spreadRadius);
                    float offsetX = Mathf.Cos(angle) * radius;
                    float offsetZ = Mathf.Sin(angle) * radius;
                    spawnPos = new Vector3(centerPos.x + offsetX, centerPos.y, centerPos.z + offsetZ);
                    attempts++;
                } while (!IsPositionValid(spawnPos, spawnedPositions, minDistance) && attempts < maxAttempts);

                spawnedPositions.Add(spawnPos);

                GameObject go = Instantiate(propPrefab, parent);
                go.transform.position = spawnPos;
                go.GetComponent<SpriteRenderer>().sprite = propSprites[Random.Range(0, propSprites.Length)];
            }
        }

        Debug.Log($"Spawned props on {occupiedTiles.Count} tiles.");
    }

    // Check if the new position is at least minDistance away from existing positions
    private bool IsPositionValid(Vector3 pos, List<Vector3> existing, float minDist)
    {
        foreach (var e in existing)
        {
            if (Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(e.x, e.z)) < minDist)
                return false;
        }
        return true;
    }
}
