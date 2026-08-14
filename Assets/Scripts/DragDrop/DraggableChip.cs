using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A uGUI drag-drop fixture chip: follows the pointer while dragged, and drops
/// raycasts THROUGH itself (blocksRaycasts=false during drag) so a DropZone beneath
/// it receives OnDrop. Used to prove the observe recorder captures a drag gesture.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(RectTransform))]
public class DraggableChip : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rt;
    private CanvasGroup _cg;
    private Canvas _canvas;
    private Vector2 _startAnchored;

    /// <summary>Set by a DropZone that accepted this chip — suppresses the snap-back.</summary>
    public bool Dropped { get; set; }

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _cg = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
        _startAnchored = _rt.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Dropped = false;
        _cg.blocksRaycasts = false; // let the raycast reach the drop zone beneath
    }

    public void OnDrag(PointerEventData eventData)
    {
        float scale = _canvas != null ? _canvas.scaleFactor : 1f;
        _rt.anchoredPosition += eventData.delta / Mathf.Max(scale, 0.0001f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cg.blocksRaycasts = true;
        if (!Dropped)
            _rt.anchoredPosition = _startAnchored; // no zone accepted it → snap back
    }
}
