using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A uGUI drop zone: accepts a DraggableChip on OnDrop, snaps it to the zone centre
/// (WITHOUT reparenting — so the chip keeps its stable hierarchy path), and writes a
/// readable OUTCOME to a Text ("empty" → "placed") so a replay can pin and gate an
/// outcome assertion on the drop actually happening.
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

        // Snap the chip onto the zone WITHOUT reparenting — both are siblings under the
        // Canvas, so matching anchoredPosition centres it while its path stays stable.
        var chipRt = chip.GetComponent<RectTransform>();
        var zoneRt = (RectTransform)transform;
        chip.Dropped = true;
        chipRt.anchoredPosition = zoneRt.anchoredPosition;
        Placed = true;
        SetOutcome("placed");
    }

    private void SetOutcome(string value)
    {
        if (outcomeText != null)
            outcomeText.text = value;
    }
}
