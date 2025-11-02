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
    private Color originalColor;
    private Canvas parentCanvas;
    public int correctZoneId;

    [Tooltip("ѕравильна€ ли карта, дл€ любой миниигры")]
    public bool isCorrect;
    [Tooltip("ѕримечание дл€ карты, дл€ любой миниигры")]
    public string note;

    public void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        originalSize = GetComponent<RectTransform>().sizeDelta;
        container = GetComponentInParent<cardContainerScript>();
        container.containedCard = this;
        originalColor = image.color;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        GetComponent<RectTransform>().sizeDelta = originalSize;
        GetComponentInChildren<TextMeshProUGUI>().enabled = false;   
        parentAfterDrag = transform.parent;
        cardContainerScript originalContainer = parentAfterDrag.GetComponent<cardContainerScript>();
        if (originalContainer != null)
        {
            originalContainer.ResetHighlight();
        }
        transform.SetParent(parentCanvas.transform);
        transform.SetAsLastSibling();
        transform.localScale *= 1.2f;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.8f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        container.containedCard = null;
        Transform dropTarget = eventData.pointerCurrentRaycast.gameObject.transform;
        cardContainerScript targetContainer = dropTarget.GetComponentInParent<cardContainerScript>();
        transform.SetParent(parentAfterDrag);
        container = parentAfterDrag.gameObject.GetComponent<cardContainerScript>();
        container.containedCard = this;
        image.raycastTarget = true;
        transform.localScale *= 0.8333f;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        if (targetContainer == null)
        {
            var text = GetComponentInChildren<TextMeshProUGUI>();
            text.enabled = true;
        }
    }
}
