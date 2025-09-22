using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class CanvasSwitcher1 : MonoBehaviour
{
    [System.Serializable]
    public class ButtonToCanvasMapping
    {
        public List<Button> buttons;    
        public GameObject targetCanvas;   
    }

    [Header("Настройка переходов: Кнопки - Целевой Canvas")]
    public List<ButtonToCanvasMapping> mappings;

    [Header("Начальный Canvas (укажи вручную!)")]
    public GameObject initialCanvas;

    public GameObject currentActiveCanvas;

    private Dictionary<string, GameObject> canvasMap = new Dictionary<string, GameObject>();
    void Start()
    {
        if (mappings == null || mappings.Count == 0)
        {
            Debug.LogError("CanvasSwitcher: список переходов пуст!");
            return;
        }

        foreach (var mapping in mappings)
        {
            if (mapping.buttons == null || mapping.targetCanvas == null) continue;

            foreach (var button in mapping.buttons)
            {
                if (button == null) continue;

                var target = mapping.targetCanvas;
                button.onClick.AddListener(() => SwitchToCanvas(target));
            }
        }
        // Заполняем словарь всеми Canvas'ами
        foreach (var mapping in mappings)
        {
            if (mapping.targetCanvas != null)
            {
                string name = mapping.targetCanvas.name;
                if (!canvasMap.ContainsKey(name))
                {
                    canvasMap[name] = mapping.targetCanvas;
                }
            }
        }

        // Добавляем начальный Canvas, если он ещё не добавлен
        if (initialCanvas != null && !canvasMap.ContainsKey(initialCanvas.name))
        {
            canvasMap[initialCanvas.name] = initialCanvas;
        }
    }


    public void SwitchToCanvas(GameObject targetCanvas)
    {
        if (targetCanvas == null) return;

        // Сохраняем предыдущий Canvas в историю (если он был)
        if (currentActiveCanvas != null && currentActiveCanvas != targetCanvas)
        {
            SavePreviousCanvas(currentActiveCanvas.name);
            currentActiveCanvas.SetActive(false);
        }

        targetCanvas.SetActive(true);
        currentActiveCanvas = targetCanvas;

        Debug.Log($"Переключились на Canvas: {targetCanvas.name}");
    }

    // Новый метод — сохраняет Canvas в историю
    private void SavePreviousCanvas(string canvasName)
    {
        // Найдём BackButtonManager в сцене
        var backButton = FindObjectOfType<backButtonScript1>();
        if (backButton != null && backButton.saveLoadManager != null)
        {
            var saveData = backButton.saveLoadManager.loadData();
            saveData.lastCanvases.Add(canvasName);
            backButton.saveLoadManager.saveData(saveData);
            Debug.Log($" Сохранили в историю: {canvasName}");
        }
    }
    public GameObject GetCanvasByName(string name)
    {
        if (canvasMap.TryGetValue(name, out GameObject canvas))
        {
            return canvas;
        }
        Debug.LogWarning($"Canvas с именем '{name}' не найден в canvasMap!");
        return null;
    }
}