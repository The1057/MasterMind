using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class cardContainerScript : MonoBehaviour, IDropHandler
{
    public cardScript containedCard;
    public bool hasCard = false;
    public bool disableTextOnDrop = false;
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            cardScript card = dropped.GetComponent<cardScript>();
            card.parentAfterDrag = transform;
            containedCard = card;
            if (disableTextOnDrop)
            {
                dropped.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                dropped.GetComponentInChildren<TextMeshProUGUI>().enabled = true;
            }
        }
    }
    void Start()
    {
        
    }
}
