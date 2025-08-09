using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InputField))]
public class InputFieldBorderHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Image borderImage; // —юда перетащи саму рамку (Image)
    public Color normalBorderColor = Color.gray;
    public Color focusedBorderColor = Color.black;

    void Start()
    {
        if (borderImage != null)
            borderImage.color = normalBorderColor;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (borderImage != null)
            borderImage.color = focusedBorderColor;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (borderImage != null)
            borderImage.color = normalBorderColor;
    }
}
