using UnityEngine;

public class Collectalbe : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] Item item;
    [SerializeField] int amount;
    [SerializeField] TileObject tile_obj;
    
    // Collect the object by clicking on it
    private void OnMouseUp()
    {
        if (tile_obj.active == false) return;
        GameManager.instance.inventory_manager.StartCollect(item, amount, transform.position);
        this.gameObject.SetActive(false);
    }
}
