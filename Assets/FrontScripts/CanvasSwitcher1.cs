using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public enum canvasSwitchAttribute
{
    disableMoneyDisplay = 0,
    enableTheory = 1,
    enableTest = 2
}

public class CanvasSwitcher1 : MonoBehaviour
{
    [System.Serializable]
    public class ButtonToCanvasMapping
    {
        public List<Button> buttons;    
        public GameObject targetCanvas;   
        public List<canvasSwitchAttribute> switchAttributes;
    }
    [Header("Объект для отображения валют")]
    public GameObject moneyDisplay;

    [Header("Настройка переходов: Кнопки - Целевой Canvas")]
    public List<ButtonToCanvasMapping> mappings;

    [Header("Начальный Canvas (укажи вручную!)")]
    public GameObject initialCanvas;

    public GameObject currentActiveCanvas;

    private Dictionary<string, GameObject> canvasMap = new Dictionary<string, GameObject>();

    [Header("Canvas List")]
    public List<GameObject> lastCanvases;
    public backButtonMode mode;
    public List<int> testIndList = new List<int>();
    TestManager2 manager;
    CanvasSequenceManager23 tManager;

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

        currentActiveCanvas = initialCanvas;
    }

    private void Update()
    {
        if (mode == backButtonMode.test)
        {
            if (testIndList.Count == 0 || manager.currentQuestion != testIndList.Last())
            {
                testIndList.Add(manager.currentQuestion);
            }
        }
        if (lastCanvases.Count > 0)
        {
            if (lastCanvases.Last() != currentActiveCanvas)
            {
                lastCanvases.Add(currentActiveCanvas);
            }
        }
        else
        {
            lastCanvases.Add(currentActiveCanvas);
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            switch (mode)
            {
                case backButtonMode.lastScene:
                    print("back");
                    loadPrevoiusCanvas();
                    break;


                case backButtonMode.theory:
                    tManager.ShowPreviousCanvas();
                    break;


                case backButtonMode.test:
                    if (testIndList.Count > 1)
                    {
                        print($"Trying to set question to index {testIndList[testIndList.Count - 2]}");
                        manager.setQuestionByIndex(testIndList[testIndList.Count - 2]);
                        testIndList.Remove(testIndList.Last());
                    }
                    break;


                default:

                    break;
            }
        }

    }
    public void SwitchToCanvas(GameObject targetCanvas)
    {
        if (targetCanvas == null) return;


        var attrubutes = getSwitchAttributesFromMappings(currentActiveCanvas);
        if (attrubutes != null)//обратные действия для текущего канваса
        {
            foreach (var attribute in attrubutes)
            {
                switch (attribute)
                {
                    case (canvasSwitchAttribute.disableMoneyDisplay):
                        enableMoneyDisplay();
                        break;

                    case (canvasSwitchAttribute.enableTheory):
                        mode = backButtonMode.lastScene;
                        break;
                    case (canvasSwitchAttribute.enableTest):
                        mode = backButtonMode.lastScene;
                        break;
                }
            }
        }

        attrubutes = getSwitchAttributesFromMappings(targetCanvas);
        if (attrubutes != null)//действия для следующего канваса
        {
            foreach (var attribute in attrubutes)
            {
                switch(attribute)
                {
                    case (canvasSwitchAttribute.disableMoneyDisplay):
                        disableMoneyDisplay();
                    break;

                    case (canvasSwitchAttribute.enableTheory):
                        mode = backButtonMode.theory;
                        tManager = targetCanvas.GetComponentInChildren<CanvasSequenceManager23>();
                    break;

                    case(canvasSwitchAttribute.enableTest):
                        mode = backButtonMode.test;
                        manager = targetCanvas.GetComponentInChildren<TestManager2>();
                    break;
                } 
            }
        }        

        // Сохраняем предыдущий Canvas в историю (если он был)
        if (currentActiveCanvas != null && currentActiveCanvas != targetCanvas)
        {
            //SavePreviousCanvas(currentActiveCanvas.name);
            currentActiveCanvas.SetActive(false);
        }

        targetCanvas.SetActive(true);
        currentActiveCanvas = targetCanvas;

        Debug.Log($"Переключились на Canvas: {targetCanvas.name}");
    }
    public void loadPrevoiusCanvas()
    {
        if (lastCanvases.Count > 1)
        {
            lastCanvases.Remove(lastCanvases.Last());
            SwitchToCanvas(lastCanvases.Last());
        }
    }
    public void disableMoneyDisplay()
    {
        moneyDisplay.SetActive(false);
    }
    public void enableMoneyDisplay()
    {
        moneyDisplay.SetActive(true);
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

    List<canvasSwitchAttribute> getSwitchAttributesFromMappings(GameObject targetCanvas)
    {
        foreach (var mapping in mappings)
        {
            if(mapping.targetCanvas != null && mapping.targetCanvas == targetCanvas)
            {
                return mapping.switchAttributes;
            }
        }
        return null;
    }
}