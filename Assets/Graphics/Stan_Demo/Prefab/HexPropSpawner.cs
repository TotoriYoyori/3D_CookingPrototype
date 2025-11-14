using UnityEngine;
using System.Collections.Generic;

public class HexPropSpawner : MonoBehaviour
{
    [Header("Prop Settings")]
    public GameObject propPrefab;         // prefab with SpriteRenderer + Billboard
    public Sprite[] propSprites;          // all your 2D sprites
    public int maxPropsPerTile = 3;       // max props per tile
    public float spreadRadius = 0.3f;     // radius around hex center to spread props
    public float minDistance = 0.1f;      // minimum distance between props

    [Header("Parenting")]
    public Transform hexTileParent;       // parent container of all hex tiles
    public Transform propParent;          // parent for spawned props

    [ContextMenu("Spawn Props on Hex Tiles")]
    public void SpawnProps()
    {
        if (propPrefab == null || propSprites.Length == 0 || hexTileParent == null)
        {
            Debug.LogWarning("Missing propPrefab, propSprites, or hexTileParent!");
            return;
        }

        // Clear previously spawned props
        if (propParent != null)
        {
            for (int i = propParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(propParent.GetChild(i).gameObject);
            }
        }

        foreach (Transform hexTile in hexTileParent)
        {
            Vector3 centerPos = hexTile.position;
            int propsOnTile = Random.Range(0, maxPropsPerTile + 1);
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

                GameObject go = Instantiate(propPrefab, propParent);
                go.transform.position = spawnPos;
                go.GetComponent<SpriteRenderer>().sprite = propSprites[Random.Range(0, propSprites.Length)];
            }
        }

        Debug.Log($"Spawned props on {hexTileParent.childCount} hex tiles.");
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
