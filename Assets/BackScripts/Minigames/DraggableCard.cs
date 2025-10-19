using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private DragDropGame gameManager;
    private StatementData data;
    private Coroutine currentMovement;

    public void Initialize(DragDropGame manager, StatementData statement)
    {
        gameManager = manager;
        data = statement;
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalPosition = rectTransform.anchoredPosition;
        TMP_Text textComponent = GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = statement.statement;
        }
        else
        {
            Debug.LogWarning($"TMP_Text component not found on DraggableCard {gameObject.name}");
        }

        canvasGroup.blocksRaycasts = true; // Убедимся, что карточка готова к взаимодействию
        Debug.Log($"Initialized card {gameObject.name} with statement: {statement.statement}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"OnBeginDrag called on {gameObject.name}");

        // Если есть активная корутина, останавливаем её
        if (currentMovement != null)
        {
            StopCoroutine(currentMovement);
            currentMovement = null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false; // Отключаем, чтобы видеть DropZone
        }
        rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvasGroup != null && canvasGroup.blocksRaycasts)
        {
            Debug.LogWarning($"OnDrag called while blocksRaycasts is true on {gameObject.name}!");
            return;
        }

        // Делим на scaleFactor Canvas, чтобы движение было корректным
        float scaleFactor = GetComponentInParent<Canvas>().scaleFactor;
        rectTransform.anchoredPosition += eventData.delta / scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"OnEndDrag called on {gameObject.name}");

        DropZone dropZone = GetDropZoneUnderPointer(eventData);
        if (dropZone != null)
        {
            Debug.Log($"Dropping on zone: {dropZone.gameObject.name}");
            if (gameManager.OnCardDropped(this, dropZone))
            {
                Debug.Log($"Card dropped successfully on {dropZone.gameObject.name}");
            }
            else
            {
                Debug.Log($"Card drop rejected by gameManager on {dropZone.gameObject.name}");
                ReturnToStart();
            }
        }
        else
        {
            Debug.Log($"No DropZone found under pointer for {gameObject.name}, returning to start");
            ReturnToStart();
        }
    }

    private DropZone GetDropZoneUnderPointer(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        Debug.Log($"RaycastAll found {results.Count} results");

        foreach (var result in results)
        {
            DropZone zone = result.gameObject.GetComponent<DropZone>();
            if (zone != null)
            {
                Debug.Log($"Found DropZone: {result.gameObject.name}");
                return zone;
            }
        }
        Debug.Log("No valid DropZone found");
        return null;
    }

    public void SnapToPosition(Vector2 targetPosition, Action onComplete)
    {
        if (currentMovement != null)
        {
            StopCoroutine(currentMovement);
        }
        currentMovement = StartCoroutine(SmoothSnap(targetPosition, onComplete));
    }

    IEnumerator SmoothSnap(Vector2 targetPosition, Action onComplete)
    {
        Debug.Log($"SmoothSnap started for {gameObject.name} to position {targetPosition}");
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }

        Vector2 startPos = rectTransform.anchoredPosition;
        float duration = 0.1f; // Быстрое притягивание
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true; // Восстанавливаем взаимодействие
        }

        Debug.Log($"SmoothSnap completed for {gameObject.name}");
        onComplete?.Invoke();
        currentMovement = null;
    }

    public void ReturnToStart()
    {
        if (currentMovement != null)
        {
            StopCoroutine(currentMovement);
        }
        currentMovement = StartCoroutine(SmoothReturnToStart());
    }

    IEnumerator SmoothReturnToStart()
    {
        Debug.Log($"SmoothReturnToStart started for {gameObject.name}");
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }

        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = originalPosition;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true; // Восстанавливаем взаимодействие
        }

        Debug.Log($"SmoothReturnToStart completed for {gameObject.name}");
        currentMovement = null;

        // Сбрасываем isProcessingFeedback в DragDropGame
        if (gameManager != null)
        {
            gameManager.SetProcessingFeedback(false);
        }
        else
        {
            Debug.LogError($"gameManager is null in SmoothReturnToStart for {gameObject.name}");
        }
    }
}