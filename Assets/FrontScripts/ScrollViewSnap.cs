using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScrollViewSnap : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    float snapTo;    
    public RectTransform content;
    public List<RectTransform> items = new();
    ScrollRect Scroll;
    public float snapTime;
    bool dragging;

    private void Start()
    {
        content =   transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        for (int i = 0; i < content.childCount; i++)
        {
            items.Add(content.GetChild(i).GetComponent<RectTransform>());
        }
        Scroll = GetComponent<ScrollRect>();
    }

    [ContextMenu("DebugSnap")]
    public void debugSnap()
    {
        snapTo = 0;
        startSnap();
    }
    public void startSnap()
    {
        StartCoroutine(snap());
    }

    IEnumerator snap()
    {
        float t = 0;
        Vector3 newPos = content.localPosition;
        Vector3 oldPos = content.localPosition;
        newPos.y = snapTo;
        do
        {
            print($"Snapping to {newPos}, time: {t/snapTime}, current pos: {content.localPosition}");
            t += Time.deltaTime;            
            content.localPosition = Vector3.Lerp(oldPos,newPos,t/snapTime);
            yield return new WaitForEndOfFrame();
        }
        while (t < snapTime);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = true;
        snapTo = items[0].sizeDelta.y * (items.Count-1 - (int)(Scroll.verticalNormalizedPosition * items.Count));
        StartCoroutine(snap());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = false;
    }
}
