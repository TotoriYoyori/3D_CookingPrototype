using UnityEngine;

public class Collectalbe : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] Item item;
    [SerializeField] int amount;

    [Header("refs")]
    [SerializeField] GameObject default_aura;
    [SerializeField] GameObject highlighted_aura;
    Tile host_tile;

    
    void Activate(bool is_active)
    {

    }

    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }

    // Highlight the object when a mouse is hovering over the object
    void Highlight(bool active)
    {
        default_aura.SetActive(!active);
        highlighted_aura.SetActive(active);
    }

    // Collect the object by clicking on it
    private void OnMouseUp()
    {
        GameManager.instance.inventory_manager.StartCollect(item, amount, transform.position);
        this.gameObject.SetActive(false);
    }
}
