using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Collection animation parameters")]
    [SerializeField] float pause_before_collecting;
    [SerializeField] float distance_floating_up;
    [SerializeField] float time_floating_up;
    List<GameObject> anim_gameobjects = new List<GameObject>();

    [Header("refs")]
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject inventory_object; // << do this class (button and slots and stuff)
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.inventory_manager = this;
    }

    public IEnumerator Collect(Item item, int amount, Vector3 world_position)
    {
        Debug.Log("InventoryManager: " + item.item_name + " ("+amount+") has been collected");

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

        // 2) Item stays there for a bit
        yield return new WaitForSeconds(pause_before_collecting);

        // 3) Item flies towards the inventory slot


        // 4) Item is popping in the inventory slot
        // 5) Item is added to the inventory slot

        yield return null;
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
