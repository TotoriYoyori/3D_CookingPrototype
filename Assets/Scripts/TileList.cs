using System.Collections.Generic;
// using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class TileList : MonoBehaviour
{
    [Header("Drafting pool tiles")]
    [SerializeField] List<Tile> grassland_pool = new List<Tile>();
    [SerializeField] List<Tile> wetland_pool = new List<Tile>();
    [SerializeField] List<Tile> coastland_pool = new List<Tile>();
    [SerializeField] List<Tile> underland_pool = new List<Tile>();
    [SerializeField] List<Tile> forageland_pool = new List<Tile>();
    [SerializeField] List<Tile> unique_pool = new List<Tile>();

    List<Tile>[] drafting_pools = new List<Tile>[5];

    private void Start()
    {
        CombineDraftimgPools();
    }

    void CombineDraftimgPools()
    {
        drafting_pools[0] = grassland_pool;
        drafting_pools[1] = wetland_pool;
        drafting_pools[2] = coastland_pool;
        drafting_pools[3] = underland_pool;
        drafting_pools[4] = forageland_pool;
    }

    public Tile GetDraftTile(Biome biome, Rarity rarity)
    {
        // Creating a list to store all tiles of needed rarity
        List<Tile> filtered_tiles = new List<Tile>();

        foreach (Tile tile in drafting_pools[(int)biome]) // Filter tiles of chosen rarity
        {
            if (tile.rarity == rarity) filtered_tiles.Add(tile);
        }

        if (filtered_tiles.Count < 1)
        {
            Debug.Log("TileList: desired tile couldnt be found.");
            return null; // return nothing if couldnt find needed tile
        }

        // Picking random tile from filtered list and return it
        Tile tile_to_return = filtered_tiles[Random.Range(0, filtered_tiles.Count)];
        Debug.Log("TileList: tile added to drafting choice: " + tile_to_return.t_name);
        return tile_to_return;
    }

    public Tile GetSpecificTile(string name)
    {
        foreach (Tile tile in unique_pool) // finding specified tile
        {
            if (tile.t_name == name) return tile;
        }

        // if specified tile couldnt be found, return null
        return null;
    }
}
