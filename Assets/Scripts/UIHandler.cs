using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public enum TextTagPlacement
{
    ABOVE,
    BELOW,
    LEFT,
    RIGHT,
}

public class UIHandler : MonoBehaviour
{
    [Header("Text tag")]
    [SerializeField] RectTransform text_tag_obj;
    [SerializeField] TextMeshProUGUI text_tag;
    [SerializeField] float text_tag_distance_hor;
    [SerializeField] float text_tag_distance_vert;

    private void Start()
    {
        GameManager.instance.UI = this;
    }

    // Create a text box next to a chosen object
    public void ShowTextTag(string text, TextTagPlacement placement, GameObject obj_to_tag)
    {
        // Enabling text tag and putting the correct text in
        text_tag_obj.gameObject.SetActive(true);
        text_tag.text = text;

        // Placing the text tag on the desired object
        bool tag_UI_obj = (obj_to_tag.TryGetComponent<RectTransform>(out RectTransform rt)) ? true : false;
        text_tag_obj.position = (tag_UI_obj) ? obj_to_tag.transform.position : Camera.main.WorldToScreenPoint(obj_to_tag.transform.position);

        // Positioning the text tag correctly
        Vector3 pos_offset = new Vector3(0,0,0);
        switch (placement)
        {
            case TextTagPlacement.ABOVE:
                text_tag.alignment = TextAlignmentOptions.Center;
                pos_offset.y += text_tag_distance_hor;
                pos_offset.y += text_tag_obj.rect.height / 2;
                break;
            case TextTagPlacement.BELOW:
                text_tag.alignment = TextAlignmentOptions.Center;
                pos_offset.y -= text_tag_distance_hor;
                pos_offset.y -= text_tag_obj.rect.height / 2;
                break;
            case TextTagPlacement.LEFT:
                text_tag.alignment = TextAlignmentOptions.Left;
                pos_offset.x -= text_tag_distance_vert;
                pos_offset.x -= text_tag_obj.rect.width / 2;
                break;
            case TextTagPlacement.RIGHT:
                text_tag.alignment = TextAlignmentOptions.Right;
                pos_offset.x += text_tag_distance_vert;
                pos_offset.x += text_tag_obj.rect.width / 2;
                break;
        }

        text_tag_obj.position += pos_offset;
    }

    public void HideTextTag()
    {
        text_tag_obj.gameObject.SetActive(false);
    }
}
