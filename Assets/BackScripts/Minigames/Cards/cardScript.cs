using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

public class cardScript : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public cardContainerScript container;
    public Transform parentAfterDrag;
    public UnityEngine.UI.Image image;
    Vector2 originalSize;

    [Tooltip("ѕравильна€ ли карта, дл€ любой миниигры")]
    public bool isCorrect;
    [Tooltip("ѕримечание дл€ карты, дл€ любой миниигры")]
    public string note;

    public void Start()
    {
        originalSize = GetComponent<RectTransform>().sizeDelta;
        container = GetComponentInParent<cardContainerScript>();
        container.containedCard = this;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        GetComponent<RectTransform>().sizeDelta = originalSize;
        GetComponentInChildren<TextMeshProUGUI>().enabled = false;   
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        transform.localScale *= 1.2f;
        image.color = new Color(1,1,1,0.8f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        container.containedCard = null;
        transform.SetParent(parentAfterDrag);
        container = parentAfterDrag.gameObject.GetComponent<cardContainerScript>();
        container.containedCard = this;
        image.raycastTarget = true;
        transform.localScale *= 0.8333f;
        image.color = new Color(1, 1, 1, 1f);
    }
}
