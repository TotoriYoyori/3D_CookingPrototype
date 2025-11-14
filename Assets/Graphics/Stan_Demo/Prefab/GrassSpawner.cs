using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GrassSpawner : MonoBehaviour
{
    [Header("Grass Settings")]
    public GameObject grassPrefab;       // prefab with SpriteRenderer + Billboard
    public Sprite[] grassSprites;        // multiple grass variations
    public int maxGrassPerTile = 20;     
    public float spreadRadius = 0.2f;   
    public float minDistance = 0.05f;   
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f); // min/max scale

    [Header("Tilemap Reference")]
    public Tilemap tilemap;

    [Header("Parenting")]
    public Transform parent;            

    [ContextMenu("Spawn Grass")]
    public void SpawnGrass()
    {
        if (grassPrefab == null || tilemap == null || grassSprites.Length == 0)
        {
            Debug.LogWarning("Missing grassPrefab, tilemap, or grassSprites!");
            return;
        }

        // --- Clear previously spawned grass ---
        if (parent != null)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                DestroyImmediate(parent.GetChild(i).gameObject);
        }

        // --- Collect occupied tiles ---
        List<Vector3Int> occupiedTiles = new List<Vector3Int>();
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
                occupiedTiles.Add(pos);
        }

        if (occupiedTiles.Count == 0)
        {
            Debug.LogWarning("No tiles found on tilemap to spawn grass!");
            return;
        }

        // --- Spawn grass ---
        foreach (Vector3Int tilePos in occupiedTiles)
        {
            int grassCount = Random.Range(maxGrassPerTile / 2, maxGrassPerTile + 1);
            Vector3 centerPos = tilemap.GetCellCenterWorld(tilePos);
            List<Vector3> spawnedPositions = new List<Vector3>();

            for (int i = 0; i < grassCount; i++)
            {
                Vector3 spawnPos;
                int attempts = 0;
                const int maxAttempts = 20;

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

                GameObject go = Instantiate(grassPrefab, parent);
                go.transform.position = spawnPos;

                // --- Random sprite selection ---
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = grassSprites[Random.Range(0, grassSprites.Length)];

                // --- Random scale ---
                float scale = Random.Range(scaleRange.x, scaleRange.y);
                go.transform.localScale = new Vector3(scale, scale, 1);
            }
        }

        Debug.Log($"Spawned grass on {occupiedTiles.Count} tiles.");
    }

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
