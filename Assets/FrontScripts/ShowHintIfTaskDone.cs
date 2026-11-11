
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShowHintIfTaskDone : MonoBehaviour
{
    [Header("Объект, за которым следим")]
    [SerializeField] private GameObject targetImage;

    [Header("Последовательность подсказок")]
    [SerializeField] private GameObject[] hintObjects;

    private bool wasActive;
    private bool hintShown;
    private int currentHintIndex = -1;

    private RectTransform currentHintRect;
    private Canvas currentHintCanvas;

    private void Start()
    {
        if (targetImage == null || hintObjects == null || hintObjects.Length == 0)
        {
            Debug.LogError("ShowHintIfTaskDone: Не назначены объекты!");
            enabled = false;
            return;
        }

        // Запоминаем начальное состояние картинки
        wasActive = targetImage.activeInHierarchy;

        // Скрываем все подсказки при запуске
        foreach (GameObject hint in hintObjects)
        {
            if (hint != null)
                hint.SetActive(false);
        }
    }

    private void Update()
    {
        // Отслеживаем отключение указанной картинки
        bool currentState = targetImage.activeInHierarchy;

        if (wasActive && !currentState && !hintShown)
        {
            hintShown = true;
            ShowHint(0);
        }

        wasActive = currentState;

        // Проверяем нажатия, только если подсказка отображается
        if (currentHintIndex < 0)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            CheckClick(Input.mousePosition);
        }
        else if (Input.touchCount > 0 &&
                 Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckClick(Input.GetTouch(0).position);
        }
    }

    private void ShowHint(int index)
    {
        // Если подсказки закончились — закрываем цепочку
        if (index >= hintObjects.Length)
        {
            currentHintIndex = -1;
            currentHintRect = null;
            currentHintCanvas = null;
            return;
        }

        // Пропускаем незаполненные элементы
        if (hintObjects[index] == null)
        {
            ShowHint(index + 1);
            return;
        }

        currentHintIndex = index;

        GameObject hint = hintObjects[index];
        hint.SetActive(true);

        currentHintRect = hint.GetComponent<RectTransform>();
        currentHintCanvas = hint.GetComponentInParent<Canvas>();
    }

    private void CheckClick(Vector2 position)
    {
        if (currentHintRect == null)
            return;

        Camera cam = null;

        if (currentHintCanvas != null &&
            currentHintCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = currentHintCanvas.worldCamera;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(
            currentHintRect, position, cam))
        {
            // Скрываем текущую подсказку
            hintObjects[currentHintIndex].SetActive(false);

            // Показываем следующую
            ShowHint(currentHintIndex + 1);
        }
    }
}
