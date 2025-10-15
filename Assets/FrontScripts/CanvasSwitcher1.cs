using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine.InputSystem.Composites;

public enum canvasSwitchAttribute
{
    disableMoneyDisplay = 0,
    enableTheory = 1,
    enableTest = 2,
    animationPlay = 3,
    disableBack = 4,
    addNextTheory = 5,
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
    [Header("Объект для отображения валют и фона")]
    public GameObject moneyDisplay;
    public GameObject back;

    [Header("Список канвасов, на которые нельзя перейти кнопкой назад")]
    public List<GameObject> canvasBlackList;

    [Header("Список канвасов с теорией и начальный")]
    public List<GameObject> canvasTheory;
    public GameObject firstTheory;

    [Header("Настройка переходов: Кнопки - Целевой Canvas")]
    public List<ButtonToCanvasMapping> mappings;

    [Header("Начальный Canvas")]
    public GameObject initialCanvas;

    public GameObject currentActiveCanvas;

    private Dictionary<string, GameObject> canvasMap = new Dictionary<string, GameObject>();

    [Header("Canvas List")]
    public List<GameObject> lastCanvases;
    public backButtonMode backButtonMode;
    public List<int> testIndList = new List<int>();
    TestManager2 manager;
    CanvasSequenceManager23 tManager;
    public Animator canvasAnimator;

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
        if (backButtonMode == backButtonMode.test)
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
                if (!isInCanvasBlacklist(currentActiveCanvas))
                {
                    lastCanvases.Add(currentActiveCanvas);                                      
                }
            }
        }
        else
        {
            lastCanvases.Add(null);            
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            switch (backButtonMode)
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

        if (currentActiveCanvas != null && currentActiveCanvas != targetCanvas)
        {
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
                            backButtonMode = backButtonMode.lastScene;
                            break;
                        case (canvasSwitchAttribute.enableTest):
                            backButtonMode = backButtonMode.lastScene;
                            testIndList.Clear();
                            break;
                        case canvasSwitchAttribute.animationPlay:
                            playAnimationOnExit();
                            break;
                        case canvasSwitchAttribute.disableBack:
                            enableBack();
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
                            backButtonMode = backButtonMode.theory;
                            tManager = targetCanvas.GetComponentInChildren<CanvasSequenceManager23>();
                        break;

                        case(canvasSwitchAttribute.enableTest):
                            backButtonMode = backButtonMode.test;
                            manager = targetCanvas.GetComponentInChildren<TestManager2>();
                        break;
                        case canvasSwitchAttribute.animationPlay:
                            playAnimationOnEnter();
                            break;
                        case canvasSwitchAttribute.disableBack:
                            disableBack();
                            break;
                    } 
                }
            }        
       
            //SavePreviousCanvas(currentActiveCanvas.name);
            currentActiveCanvas.SetActive(false);
        }

        targetCanvas.SetActive(true);
        currentActiveCanvas = targetCanvas;

        Debug.Log($"Переключились на Canvas: {targetCanvas.name}");
    }
    public void loadPrevoiusCanvas()
    {
        GameObject previousCanvas = null;
        if (lastCanvases.Count > 1)
        {
            // Получаем предпоследний Canvas (на который хотим вернуться)
            previousCanvas = lastCanvases[lastCanvases.Count - 2];
            // Удаляем ПОСЛЕДНИЙ элемент (текущий)
            lastCanvases.RemoveAt(lastCanvases.Count - 1);

            // Переходим на предыдущий
            SwitchToCanvas(previousCanvas);
            
        }
    }
    public void disableMoneyDisplay() { moneyDisplay.SetActive(false); }
    public void enableMoneyDisplay()  { moneyDisplay.SetActive(true);  }
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

    bool isInCanvasBlacklist(GameObject targetCanvas)
    {
        foreach (var canvas in canvasBlackList)
        {
            if(targetCanvas==canvas) return true;
        }
        return false;
    }

    void playAnimationOnExit()  { canvasAnimator.CrossFade("ValuteDown", 0.23f); }
    void playAnimationOnEnter() { canvasAnimator.CrossFade("ValuteUp", 0.23f);   }
    void enableBack()  { back.SetActive(true);  }
    void disableBack() { back.SetActive(false); }

    public void addNextTheory(int numTheory)
    {
        // Make sure the index is valid
        if (numTheory >= 0 && numTheory <= canvasTheory.Count)
        {
            if (numTheory == 0) {
                CanvasGroup firstT = firstTheory.GetComponent<CanvasGroup>();
                firstT.alpha = 0.23f;
            }
            if (numTheory > 0)
            {
                GameObject previousCanvas = canvasTheory[numTheory - 1];

                // Get the CanvasGroup component from the previous canvas
                CanvasGroup previousCanvasGroup = previousCanvas.GetComponent<CanvasGroup>();

                // If a CanvasGroup component exists, change its alpha
                if (previousCanvasGroup != null)
                {
                    previousCanvasGroup.alpha = 0.23f;
                }
                else
                {
                    Debug.LogWarning("CanvasGroup component not found on the previous canvas! Cannot change transparency.");
                }
            }

            // Activate the new theory canvas
            canvasTheory[numTheory].SetActive(true);
        }
        else
        {
            Debug.LogWarning($"Index {numTheory} is out of bounds for the canvasTheory list.");
        }
    }
}