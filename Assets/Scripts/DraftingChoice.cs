using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DraftingChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Tile tile;
    [SerializeField] Image sprite;
    [SerializeField] TextMeshProUGUI tile_name;
    [SerializeField] TextMeshProUGUI tile_description;
    [SerializeField] Vector3 default_size;
    Vector3 hovered_size;
    [SerializeField] DraftingWindow drafting_window;

    private void Start()
    { 
        hovered_size = default_size * 1.2f;
    }

    public void Init(Tile stored_tile)
    {
        sprite.transform.localScale = default_size;
        tile = stored_tile;
        sprite.sprite = tile.sprite.sprite;
        tile_name.text = tile.t_name;
        tile_description.text = "[" + tile.t_description + "]";
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHover(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Choosing tile to place by clicking on it
        GameManager.instance.tile_manager.PlaceTile(tile);

        // Deactivating the draft after choosing next tile
        drafting_window.DeactivateDraft();
    }

    void OnHover(bool mouse_over)
    {
        // Highlighting with scale (juice)
        sprite.transform.localScale = (mouse_over) ? hovered_size : default_size;

        // Update the preview
        GameManager.instance.tile_manager.preview.sprite = (mouse_over) ? sprite.sprite : null;
    }
}
