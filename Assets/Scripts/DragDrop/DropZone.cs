using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A uGUI drop zone: accepts a DraggableChip on OnDrop, REPARENTS it under the zone
/// (so the chip's hierarchy path changes — the reparent case the gesture-time path
/// capture must handle), and writes a readable OUTCOME to a Text ("empty" → "placed")
/// so a replay can pin and gate an outcome assertion on the drop actually happening.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DropZone : MonoBehaviour, IDropHandler
{
    public Text outcomeText;

    public bool Placed { get; private set; }

    private void Start()
    {
        SetOutcome("empty");
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableChip chip = eventData.pointerDrag != null
            ? eventData.pointerDrag.GetComponent<DraggableChip>()
            : null;
        if (chip == null)
            return;

        // Reparent the chip UNDER the zone and centre it — its path becomes
        // /Canvas/DropZone/Chip, so a stop-time locator would be wrong; the gesture-time
        // capture must record the pre-drop /Canvas/Chip.
        var chipRt = chip.GetComponent<RectTransform>();
        chip.Dropped = true;
        chipRt.SetParent(transform, false);
        chipRt.anchoredPosition = Vector2.zero;
        Placed = true;
        SetOutcome("placed");
    }

    private void SetOutcome(string value)
    {
        if (outcomeText != null)
            outcomeText.text = value;
    }
}
