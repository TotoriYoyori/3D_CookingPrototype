using Unity.VisualScripting;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    [Header("refs")]
    [SerializeField] GameObject default_aura;
    [SerializeField] GameObject highlighted_aura;

    [Header("params")]
    [HideInInspector] public bool active;

    public void Activate(bool is_active)
    {
        active = is_active;
        default_aura.SetActive(is_active);
        highlighted_aura.SetActive(false);
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
    void Highlight(bool is_highlighted)
    {
        if (active == false) return;

        default_aura.SetActive(!is_highlighted);
        highlighted_aura.SetActive(is_highlighted);
    }

}
