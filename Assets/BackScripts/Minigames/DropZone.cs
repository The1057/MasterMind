using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropZone : MonoBehaviour
{
    private Image image;
    private Color originalColor;

    // Ожидаемая форма для этой зоны (задается в DragDropGame)
    [HideInInspector] public LegalForm ExpectedForm;

    void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError($"No Image component found on DropZone {gameObject.name}");
        }
        originalColor = image != null ? image.color : Color.white;
    }

    void Start()
    {
        // Гарантируем, что цвет сброшен
        ResetColor();
    }

    // Метод для установки ожидаемой формы из DragDropGame
    public void SetExpectedForm(LegalForm form)
    {
        ExpectedForm = form;
        Debug.Log($"DropZone {gameObject.name} set to expect {form}");
    }

    public void OnDrop(DraggableCard card)
    {
        if (card == null)
        {
            Debug.LogError($"OnDrop called with null card on {gameObject.name}");
            return;
        }

        Debug.Log($"OnDrop called on DropZone {gameObject.name} with card {card.gameObject.name}");

        // Сообщаем GameManager'у, что карточка сброшена
        DragDropGame game = FindFirstObjectByType<DragDropGame>();
        if (game != null)
        {
            bool success = game.OnCardDropped(card, this);
            Debug.Log($"OnCardDropped result: {success} for {gameObject.name}");
        }
        else
        {
            Debug.LogError("DragDropGame not found in scene!");
        }
    }

    public IEnumerator FlashColor(Color flashColor)
    {
        if (image == null)
        {
            Debug.LogWarning($"Cannot flash color: Image component is null on {gameObject.name}");
            yield break;
        }

        Debug.Log($"FlashColor started on {gameObject.name} with color {flashColor}");
        Color startColor = image.color;
        Color targetColor = new Color(flashColor.r, flashColor.g, flashColor.b, 0.5f); // Полупрозрачный

        float duration = 0.3f;
        float elapsed = 0f;

        // Вспышка до цвета
        while (elapsed < duration)
        {
            image.color = Color.Lerp(startColor, targetColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        image.color = targetColor;

        // Возврат к оригиналу
        elapsed = 0f;
        while (elapsed < duration)
        {
            image.color = Color.Lerp(targetColor, originalColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        image.color = originalColor;

        Debug.Log($"FlashColor completed on {gameObject.name}");
    }

    public void ResetColor()
    {
        if (image != null)
        {
            image.color = originalColor;
        }
    }
}