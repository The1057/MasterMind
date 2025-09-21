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

    private GameObject currentActiveCanvas;
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
    }
    

    void SwitchToCanvas(GameObject targetCanvas)
    {
        if (targetCanvas == null) return;

        if (currentActiveCanvas == null) { currentActiveCanvas = initialCanvas; }
        if (currentActiveCanvas != null && currentActiveCanvas != targetCanvas) { currentActiveCanvas.SetActive(false); }
        targetCanvas.SetActive(true);
        currentActiveCanvas = targetCanvas;
    }
}