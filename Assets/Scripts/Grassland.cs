using UnityEngine;
using System.Collections.Generic;

public enum CropType
{
    WHEAT,
    RICE,
    BOTH
}
public class Grassland : MonoBehaviour
{ 
    [Header("Configurations")]
    [SerializeField] int crop_amount_min;
    [SerializeField] int crop_amount_max;
    [SerializeField] CropType crop_type;

    [Header("prefabs and refs (Grassland)")]
    [SerializeField] GameObject tree_spots;
    List<GameObject> trees = new List<GameObject>();
    [SerializeField] GameObject tree_prefab;
    [SerializeField] GameObject crop_spots;
    List<GameObject> crops = new List<GameObject>();
    [SerializeField] GameObject[] crop_prefabs; // 0- wheat, 1- rice

    private void Start()
    {
        SpawnCrops();
        SpawnTrees();
    }

    // Creates trees on the borders of tiles (or wherever we decide)
    void SpawnTrees()
    {
        trees = GetSpots(trees, tree_spots);

        // Spawning trees
        for (int a = 0; a < trees.Count; a++)
        {
            GameObject new_tree = Instantiate(tree_prefab, trees[a].transform.position, Quaternion.identity, transform);
        }
    }

    void SpawnCrops()
    {
        crops = GetSpots(crops, crop_spots);

        int crop_amount = Random.Range(crop_amount_min, crop_amount_max + 1);

        for (int a= 0; a < crop_amount; a++)
        {
            // Decide where to place crop 
            int random_crop_spot_id = Random.Range(0, crops.Count);

            // Decide which crop if both are an option
            if (crop_type == CropType.BOTH) crop_type = (Random.Range(0, 2) == 0) ? CropType.WHEAT : CropType.RICE;

            // Spawn the crop
            GameObject new_crop = Instantiate(crop_prefabs[(int)crop_type], crops[random_crop_spot_id].transform.position, Quaternion.identity, transform);
        }
    }

    // Gets all the spot objects from spot_parent automatically and disables their SpriteRenderers
    List<GameObject> GetSpots(List<GameObject> spot_list, GameObject spot_parent)
    {
        if (spot_parent.activeSelf == false) return spot_list;

        for (int a = 0; a < spot_parent.transform.childCount; a++)
        {
            GameObject spot_child = spot_parent.transform.GetChild(a).gameObject;
            spot_list.Add(spot_child);

            spot_child.GetComponent<SpriteRenderer>().sprite = null;
        }

        return spot_list;
    }
}
