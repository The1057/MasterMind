using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageCard : MonoBehaviour, IPointerEnterHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public StageContainer container = null;
    StageRelocationGame gameManager;
    public GameObject ghost;
    public int cardIndex;
    Image thisImage;

    void Start()
    {
        gameManager = FindFirstObjectByType<StageRelocationGame>();
        container = GetComponentInParent<StageContainer>();
        ghost = Instantiate(this.gameObject);
        ghost.GetComponent<Image>().raycastTarget = false;
        ghost.transform.parent = gameManager.transform;
        ghost.SetActive(false);
        Destroy(ghost.GetComponent<StageCard>());
        thisImage = this.gameObject.GetComponent<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        container.objectBeingDragged = this.gameObject;
        ghost.SetActive(true);
        transform.SetAsLastSibling();
        Color imc = thisImage.color;
        imc.a = 0;
        thisImage.color = imc;
    }
    public void OnDrag(PointerEventData data)
    {
        ghost.transform.position = new Vector3(transform.position.x, Input.mousePosition.y, 0);
        // Do nothing
        // Apparently this interface needs to exist in order for BeginDrag and EndDrag to work,
        // but we don't actually have anything to do here
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (container.objectBeingDragged == this.gameObject) container.objectBeingDragged = null;
        ghost.SetActive(false);
        Color imc = thisImage.color;
        imc.a = 1;
        thisImage.color = imc;
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