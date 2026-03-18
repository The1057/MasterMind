using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StageCard : MonoBehaviour, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public StageContainer container = null;

    void Start()
    {
        container = GetComponentInParent<StageContainer>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        container.objectBeingDragged = this.gameObject;
    }
    public void OnDrag(PointerEventData data)
    {
        // Do nothing
        // Apparently this interface needs to exist in order for BeginDrag and EndDrag to work,
        // but we don't actually have anything to do here
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (container.objectBeingDragged == this.gameObject) container.objectBeingDragged = null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject objectBeingDragged = container.objectBeingDragged;
        if (objectBeingDragged != null && objectBeingDragged != this.gameObject)
        {
            objectBeingDragged.transform.SetSiblingIndex(this.transform.GetSiblingIndex());
        }
    }
}