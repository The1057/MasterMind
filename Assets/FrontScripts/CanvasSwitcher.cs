using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CanvasSwitcher : MonoBehaviour
{
    [System.Serializable]
    public class CanvasWithButton
    {
        public GameObject canvas;
        public Button nextButton;
    }

    [Header("Порядок Canvas'ов и их кнопки")]
    public List<CanvasWithButton> canvasSequence;

    private int currentIndex = 0;

    void Start()
    {
        if (canvasSequence.Count == 0)
        {
            Debug.LogError("CanvasSwitcher: список Canvas'ов пуст!");
            return;
        }

        // Отключаем все, кроме первого
        for (int i = 0; i < canvasSequence.Count; i++)
        {
            bool isActive = (i == currentIndex);
            canvasSequence[i].canvas.SetActive(isActive);
        }

        // Назначаем действия кнопкам
        for (int i = 0; i < canvasSequence.Count; i++)
        {
            int nextIndex = i + 1;
            if (canvasSequence[i].nextButton != null && nextIndex < canvasSequence.Count)
            {
                int capturedIndex = nextIndex; // фикс захвата переменной
                canvasSequence[i].nextButton.onClick.AddListener(() => SwitchToCanvas(capturedIndex));
            }
        }
    }

    void SwitchToCanvas(int index)
    {
        if (index >= canvasSequence.Count) return;

        // Скрываем текущий
        canvasSequence[currentIndex].canvas.SetActive(false);

        // Показываем новый
        currentIndex = index;
        canvasSequence[currentIndex].canvas.SetActive(true);
    }
}
