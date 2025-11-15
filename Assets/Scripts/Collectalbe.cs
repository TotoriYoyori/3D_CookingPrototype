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
        GameManager.instance.UI.HideTextTag();
        this.gameObject.SetActive(false);
    }

    // Text tag appearing when hovering over
    private void OnMouseEnter()
    {
        string text_to_show = item.item_name + "\n(Click to collect)";
        GameManager.instance.UI.ShowTextTag(text_to_show, TextTagPlacement.ABOVE, this.gameObject);
    }

    private void OnMouseExit()
    {
        GameManager.instance.UI.HideTextTag();
    }
}
