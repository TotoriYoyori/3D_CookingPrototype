using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Data;

public class InventoryManager : MonoBehaviour
{
    [Header("Collection animation parameters")]
    [SerializeField] float pause_before_collecting;
    [SerializeField] float distance_floating_up;
    [SerializeField] float time_floating_up;
    List<GameObject> anim_gameobjects = new List<GameObject>();

    [Header("Floating to inventory animation parameters")]
    [SerializeField] float inventory_floating_anim_time;

    [Header("refs")]
    [SerializeField] Canvas canvas;
    [SerializeField] InventoryUIHandler inventory_object; 
    [HideInInspector] public List<InventorySlot> inventory_slots = new List<InventorySlot>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.inventory_manager = this;
    }

    public IEnumerator Collect(Item item, int amount, Vector3 world_position)
    {
        Debug.Log("InventoryManager: " + item.item_name + " ("+amount+") has been collected");
        InventorySlot item_inventory_slot;

        // 0) Creating the image GameObject
        GameObject item_anim_object = GetImageForAnimation();
        item_anim_object.GetComponent<Image>().sprite = item.item_sprite;

        // 0.1) Setting the position
        Vector3 screen_position = Camera.main.WorldToScreenPoint(world_position);

        // Convert screen position to local canvas position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screen_position,
            canvas.worldCamera,
            out Vector2 local_point
        );

        // Apply to RectTransform
        RectTransform rt = item_anim_object.GetComponent<RectTransform>();
        rt.anchoredPosition = local_point;

        // 1) Item floats up above collected spot
        float t = 0;
        Vector2 end_position = new Vector2(item_anim_object.transform.position.x, item_anim_object.transform.position.y + distance_floating_up);
        Vector2 start_position = item_anim_object.transform.position;

        while (t <= time_floating_up)
        {
            t += Time.deltaTime;
            float t_clamped = Mathf.Clamp01(t / time_floating_up);

            float cool_t = 1 - (1 - t_clamped) * (1 - t_clamped); // fast >>> slow          (float cool_t = t_clamped * t_clamped; // slow >>> fast)
            item_anim_object.transform.localPosition = Vector3.Lerp(start_position, end_position, cool_t);
            yield return null;
        }
        item_anim_object.transform.localPosition = end_position;

        // 2) Item stays there for a bit
        yield return new WaitForSeconds(pause_before_collecting);

        // 3) Item flies towards the inventory slot
        item_inventory_slot = AssignInventorySlot(item);

        // no slots are available
        if (item_inventory_slot == null)
        {
            Debug.Log("InventoryManager: no slots were available for storing " + item.item_name);
            item_anim_object.SetActive(false);
            yield break;
        }

        t = 0;
        Vector3 anim_start_position = item_anim_object.transform.position;
        // Checking if the backpack is open or closed
        Vector3 anim_end_position = (inventory_object.inventory_hidden) ? inventory_object.transform.position : item_inventory_slot.transform.position;

        while (t < inventory_floating_anim_time)
        {
            t += Time.deltaTime;
            float t_clamped = Mathf.Clamp01(t / inventory_floating_anim_time);

            float cool_t = t_clamped * t_clamped; // slow >>> fast
            item_anim_object.transform.position = Vector3.Lerp(anim_start_position, anim_end_position, cool_t);
            yield return null;
        }
        item_anim_object.transform.position = anim_end_position;

        // 4) Adding the item to the inventory slot
        item_anim_object.SetActive(false);
        item_inventory_slot.StoreItem(item, amount);
        
        yield return null;
    }

    InventorySlot AssignInventorySlot(Item item)
    {
        // Checking if there is already a slot storing desired item
        for (int a = 0; a < inventory_slots.Count; a++)
        {
            if (inventory_slots[a].stored_item == item) return inventory_slots[a];
        }

        // Checking for next empty slot
        for (int a = 0; a < inventory_slots.Count; a++)
        {
            if (inventory_slots[a].stored_item == null) return inventory_slots[a];
        }

        // No slots are empty or store the same item
        return null;
        
    }

    GameObject GetImageForAnimation()
    {
        // Looking if we have a gameObject that we can reuse
        for (int a = 0; a < anim_gameobjects.Count; a++)
        {
            if (anim_gameobjects[a].activeSelf == false)
            {
                anim_gameobjects[a].SetActive(true);
                return anim_gameobjects[a];
            }
        }

        // If there's nothing to reuse we create new
        GameObject new_anim_gameobject = new GameObject("item_anim_gameobject", typeof(Image));
        new_anim_gameobject.transform.SetParent(canvas.transform, false);
        return new_anim_gameobject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
