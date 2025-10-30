using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class cardContainerScript : MonoBehaviour, IDropHandler
{
    public int zoneId;
    public cardScript containedCard;
    public bool hasCard = false;
    public bool disableTextOnDrop = false;
    public UnityEngine.UI.Image background;
    private Color defaultColor;
    public Color incorrectColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            cardScript card = dropped.GetComponent<cardScript>();
            card.parentAfterDrag = transform;
            containedCard = card;
            var text = dropped.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.enabled = !disableTextOnDrop;
        }
    }
    void Start()
    {
        background = GetComponent<Image>() ?? GetComponentInChildren<Image>();

        if (background != null)
        {
            // Сохраняем исходный цвет
            defaultColor = background.color;
        }
        else
        {
            Debug.LogWarning("Не найден Image для фона в ячейке: " + name);
        }
    }
    public void MarkAsIncorrect(bool isIncorrect)
    {
        if (background != null)
        {
            background.color = isIncorrect ? incorrectColor : defaultColor;
        }
    }
    public void ResetHighlight()
    {
        MarkAsIncorrect(false);
    }
}
