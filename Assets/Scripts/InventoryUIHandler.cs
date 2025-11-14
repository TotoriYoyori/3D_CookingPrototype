using Mono.Cecil;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Configurations")]
    [SerializeField] float slot_number; // 5 max for now
    [SerializeField] float slot_spacing_cooficient = 0.3f;

    [Header("refs")]
    [SerializeField] GameObject inventory_slot_parent;
    [SerializeField] InventorySlot inventory_slot_prefab;
    [SerializeField] InventoryManager inventory_manager;
    Vector3 default_size;
    Vector3 hovered_size;
    Vector3 clicked_size;
    [HideInInspector] public bool inventory_hidden;

    private void Start()
    {
        SpawnSlots();

        default_size = transform.localScale;
        hovered_size = default_size * 1.3f;
        clicked_size = default_size * 0.7f;
    }


    // ------- Inventory Slots -----------
    void SpawnSlots()
    {
        float slot_height = inventory_slot_prefab.GetComponent<Image>().rectTransform.rect.height;
        float slot_interval = slot_height + slot_height * slot_spacing_cooficient;

        for (int a = 0; a < slot_number; a++)
        {
            InventorySlot new_slot = Instantiate(inventory_slot_prefab, inventory_slot_parent.transform.position, Quaternion.identity, inventory_slot_parent.transform);
            float current_slot_interval = -slot_interval * a;
            new_slot.gameObject.transform.Translate(0, current_slot_interval, 0);

            // Storing the slots in the inventory manager
            inventory_manager.inventory_slots.Add(new_slot);

            // Adding ref to this to the slot
            new_slot.inventory_button = this;
        }

        // Hide the slots right after spawning
        //HideInverntory(true);
    }

    // ---- Input and juice for the inventory button --------
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnHover(false);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnHover(true);

        // Pressing the button
        HideInverntory(!inventory_hidden);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // Making the button looked clicked
        transform.localScale = clicked_size;
    }

    void OnHover(bool is_mouse_over)
    {
        transform.localScale = (is_mouse_over) ? hovered_size : default_size;
    }

    void HideInverntory(bool hide)
    {
        inventory_hidden = hide;

        inventory_slot_parent.SetActive(!hide);
    }
    // -------------------------------------------------------
}
