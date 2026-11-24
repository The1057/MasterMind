
using UnityEngine;

[DisallowMultipleComponent]
public class TaskHintController : MonoBehaviour
{
    [Header("Canvas, с которого должен произойти переход")]
    [SerializeField] private GameObject sourceCanvas;

    [Header("Объекты подсказок по порядку")]
    [SerializeField] private GameObject[] hintObjects;

    [Header("Сохранение результата")]
    [SerializeField] private bool resultSaved = false;

    [Header("Настройки")]
    [SerializeField] private string saveKey = "TaskHintsCompleted";

    private int currentIndex;
    private bool isAnimating;
    private bool sourceCanvasWasActive;

    private void Awake()
    {
        HideAllHints();

        if (sourceCanvas != null)
            sourceCanvasWasActive = sourceCanvas.activeInHierarchy;
    }

    private void Update()
    {
        if (sourceCanvas == null)
            return;

        // Проверяем, был ли активен Canvas-источник
        bool sourceIsActive = sourceCanvas.activeInHierarchy;

        if (sourceIsActive)
            sourceCanvasWasActive = true;

        // Canvas-источник выключился после перехода
        if (sourceCanvasWasActive && !sourceIsActive)
        {
            sourceCanvasWasActive = false;

            if (!resultSaved && PlayerPrefs.GetInt(saveKey, 0) == 0)
                StartHints();
        }

        // Переключение подсказок нажатием
        if (isAnimating && Input.GetMouseButtonDown(0))
        {
            if (currentIndex < hintObjects.Length &&
                hintObjects[currentIndex] != null)
            {
                hintObjects[currentIndex].SetActive(false);
            }

            currentIndex++;
            ShowNextHint();
        }
    }

    private void StartHints()
    {
        if (isAnimating || hintObjects == null || hintObjects.Length == 0)
            return;

        HideAllHints();

        currentIndex = 0;
        isAnimating = true;

        ShowNextHint();
    }

    private void ShowNextHint()
    {
        while (currentIndex < hintObjects.Length &&
               hintObjects[currentIndex] == null)
        {
            currentIndex++;
        }

        if (currentIndex >= hintObjects.Length)
        {
            FinishHints();
            return;
        }

        hintObjects[currentIndex].SetActive(true);
    }

    private void FinishHints()
    {
        isAnimating = false;

        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        resultSaved = true;

        Debug.Log("TaskHintController: Подсказки пройдены и сохранены.");
    }

    private void HideAllHints()
    {
        if (hintObjects == null)
            return;

        foreach (GameObject hint in hintObjects)
        {
            if (hint != null)
                hint.SetActive(false);
        }
    }

    // Сброс прохождения для повторного тестирования
    [ContextMenu("Сбросить сохранение подсказок")]
    private void ResetHints()
    {
        PlayerPrefs.DeleteKey(saveKey);
        PlayerPrefs.Save();

        resultSaved = false;

        Debug.Log("TaskHintController: Сохранение сброшено.");
    }
}
