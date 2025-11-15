using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("What it stores")]
    public Item stored_item;
    public int amount;

    [Header("refs")]
    [SerializeField] GameObject amount_obj;
    [SerializeField] TextMeshProUGUI amount_text;
    [SerializeField] Image stored_item_sprite;
    [HideInInspector] public InventoryUIHandler inventory_button;

    [Header("Storing animation")]
    Vector3 default_sprite_size;
    [SerializeField] float sprite_upscale_coof = 1.3f;
    [SerializeField] float upscale_animation_length = 1.5f;

    private void Start()
    {
        SetStoredItemSprite();
    }

    void SetStoredItemSprite()
    {
        default_sprite_size = stored_item_sprite.transform.localScale;
        stored_item_sprite.sprite = null;
    }

    // Adding a new item into this slot or increasing the amount if thats the same thing as already stored
    public void StoreItem(Item item, int input_amount = 1)
    {
        if (item == stored_item)
        {
            amount += input_amount;
        }
        else
        {
            stored_item = item;
            amount = input_amount;
            UpdateSprite();
        }

        StartCoroutine(StoreItemAnimation());
        UpdateAmount();

        Debug.Log("InventorySlot: slot now stores " + item.item_name + " (" + amount + ")");
    }

    // Removing anything that was stored here
    public void EmptySlot()
    {
        stored_item = null;
        stored_item_sprite.sprite = null;
        amount = 0;

        UpdateAmount();
    }
    void OnEnable()
    {
        UpdateSprite();
        UpdateAmount();
    }
    
    void UpdateSprite()
    {
        stored_item_sprite.sprite = (stored_item == null) ? null : stored_item.item_sprite;

        stored_item_sprite.enabled = (stored_item_sprite.sprite == null) ? false : true; // disables the sprite if the slot is empty
    }
    void UpdateAmount()
    {
        amount_text.text = amount.ToString();

        bool show_amount_number = amount > 0 && !inventory_button.inventory_hidden;
        amount_obj.SetActive(show_amount_number);
    }

    IEnumerator StoreItemAnimation()
    {
        // No animation if the slot is hidden
        if (inventory_button.inventory_hidden) yield break;

        // Animation for the sprite growing
        Vector3 increased_sprite_size = default_sprite_size * sprite_upscale_coof;
        float t = 0;

        while (t < upscale_animation_length)
        {
            t += Time.deltaTime;

            float t_clamped = Mathf.Clamp01(t / upscale_animation_length);

            float cool_t = 1 - (1 - t_clamped) * (1 - t_clamped); // fast >>> slow          
            stored_item_sprite.transform.localScale = Vector3.Lerp(default_sprite_size, increased_sprite_size, cool_t);
            yield return null;
        }

        // Animation for the sprite shrinking
        t = 0;

        while (t < upscale_animation_length)
        {
            t += Time.deltaTime;

            float t_clamped = Mathf.Clamp01(t / upscale_animation_length);

            float cool_t = t_clamped * t_clamped; // slow >>> fast      
            stored_item_sprite.transform.localScale = Vector3.Lerp(increased_sprite_size, default_sprite_size, cool_t);
            yield return null;
        }

        stored_item_sprite.transform.localScale = default_sprite_size; // snapping into correct size at the end
    }

    // Text tag appearing when hovering over
    public void OnPointerEnter(PointerEventData eventData)
    {
        string text_to_show = stored_item.item_name;
        GameManager.instance.UI.ShowTextTag(text_to_show, TextTagPlacement.LEFT, this.gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.UI.HideTextTag();
    }
}
