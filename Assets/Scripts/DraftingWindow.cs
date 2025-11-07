using Unity.VisualScripting;
using UnityEngine;

public class DraftingWindow : MonoBehaviour
{
    public DraftingChoice[] drafting_choices = new DraftingChoice[3];

    public void ActivateDraft(Tile[] passed_tiles, Vector3 position)
    {
        for (int a = 0; a < passed_tiles.Length; a++)
        {
            drafting_choices[a].Init(passed_tiles[a]);
        }
    }
    public void DeactivateDraft()
    {
        this.gameObject.SetActive(false);
    }
}
