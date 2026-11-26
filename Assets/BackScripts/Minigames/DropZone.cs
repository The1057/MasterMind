using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropZone : MonoBehaviour
{
    private Outline image;
    private Color originalColor;

    public int ZoneId;

    void Awake()
    {
        image = GetComponent<Outline>();
        originalColor = image != null ? image.effectColor : Color.white;
    }

    void Start()
    {
        ResetColor();
    }

    public void OnDrop(DraggableCard card)
    {

        DragDropGame game = FindFirstObjectByType<DragDropGame>();
        if (game != null)
        {
            bool success = game.OnCardDropped(card, this);
        }
    }

    public IEnumerator FlashColor(Color flashColor)
    {

        Color startColor = image.effectColor;
        Color targetColor = new Color(flashColor.r, flashColor.g, flashColor.b, 0.5f); // Полупрозрачный

        float duration = 0.3f;
        float elapsed = 0f;

        // Вспышка до цвета
        while (elapsed < duration)
        {
            image.effectColor = Color.Lerp(startColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        image.effectColor = targetColor;

        // Возврат к оригиналу
        elapsed = 0f;
        while (elapsed < duration)
        {
            image.effectColor = Color.Lerp(targetColor, originalColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        image.effectColor = originalColor;

    }

    public void ResetColor()
    {
        if (image != null)
        {
            image.effectColor = originalColor;
        }
    }
}