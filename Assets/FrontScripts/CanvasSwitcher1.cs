using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public enum canvasSwitchAttribute
{
    disableMoneyDisplay = 0,
    enableTheory = 1,
    enableTest = 2,
    animationPlay = 3,
    disableBack = 4,
    addNextTheory = 5,
    cantIn = 6
}

public class CanvasSwitcher1 : MonoBehaviour
{
    [System.Serializable]
    public class ButtonToCanvasMapping
    {
        public string label;
        public List<Button> buttons;
        public GameObject targetCanvas;
        public List<canvasSwitchAttribute> switchAttributes;
    }

    [Header("Настройки")]
    public GameObject initialCanvas;
    public GameObject moneyDisplay;
    public GameObject back;
    public List<ButtonToCanvasMapping> mappings;
    public List<GameObject> canvasBlackList;

    [Header("История (только для чтения)")]
    public GameObject currentActiveCanvas;
    public List<GameObject> lastCanvases = new List<GameObject>();

    private TestManager2 quizManager;
    private CanvasSequenceManager23 theoryManager;
    private backButtonMode currentMode = backButtonMode.lastScene;

    void Start()
    {
        foreach (var mapping in mappings)
        {
            foreach (var btn in mapping.buttons)
            {
                if (btn != null) btn.onClick.AddListener(() => SwitchToCanvas(mapping.targetCanvas));
            }
        }

        if (initialCanvas != null) SwitchToCanvas(initialCanvas);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleBackNavigation();
        }
    }

    public void SwitchToCanvas(GameObject targetCanvas)
    {
        if (targetCanvas == null || targetCanvas == currentActiveCanvas) return;

        var mapping = mappings.Find(m => m.targetCanvas == targetCanvas);
        if (mapping != null && mapping.switchAttributes.Contains(canvasSwitchAttribute.cantIn))
        {
            if (!CheckEntryConditions(targetCanvas))
            {
                Debug.Log($"[CanvasSwitcher] Вход в {targetCanvas.name} заблокирован");
                // здесь можно показать попап игроку: "Сначала выберите ..."
                return;
            }
        }

        if (currentActiveCanvas != null)
        {
            ApplyAttributes(currentActiveCanvas, false);
            if (!canvasBlackList.Contains(currentActiveCanvas))
                lastCanvases.Add(currentActiveCanvas);
            currentActiveCanvas.SetActive(false);
        }

        targetCanvas.SetActive(true);
        currentActiveCanvas = targetCanvas;
        ApplyAttributes(targetCanvas, true);
    }

    private bool CheckEntryConditions(GameObject canvas)
    {
        var validators = canvas.GetComponentsInChildren<ICanvasEntryValidator>(true);
        foreach (var v in validators)
            if (!v.CanEnter()) return false;
        return true;
    }

    private void ApplyAttributes(GameObject canvas, bool entering)
    {
        var mapping = mappings.Find(m => m.targetCanvas == canvas);
        if (mapping == null) return;

        foreach (var attr in mapping.switchAttributes)
        {
            switch (attr)
            {
                case canvasSwitchAttribute.disableMoneyDisplay:
                    moneyDisplay.SetActive(!entering); // Если входим — выключаем, если выходим — включаем
                    break;
                case canvasSwitchAttribute.enableTest:
                    currentMode = entering ? backButtonMode.test : backButtonMode.lastScene;
                    if (entering) quizManager = canvas.GetComponentInChildren<TestManager2>();
                    break;
                case canvasSwitchAttribute.enableTheory:
                    currentMode = entering ? backButtonMode.theory : backButtonMode.lastScene;
                    if (entering) theoryManager = canvas.GetComponentInChildren<CanvasSequenceManager23>();
                    break;
                case canvasSwitchAttribute.disableBack:
                    back.SetActive(!entering);
                    break;
                case canvasSwitchAttribute.cantIn:

                    break;
            }
        }
    }

    private void HandleBackNavigation()
    {
        if (currentMode == backButtonMode.test && quizManager != null)
        {
            // Если мы в тесте, логика "Назад" должна быть в самом QuizManager
            // quizManager.GoToPreviousQuestion(); 
        }
        else if (currentMode == backButtonMode.theory && theoryManager != null)
        {
            theoryManager.ShowPreviousCanvas();
        }
        else
        {
            LoadPreviousCanvas();
        }
    }

    public void LoadPreviousCanvas()
    {
        if (lastCanvases.Count > 0)
        {
            GameObject prev = lastCanvases.Last();
            lastCanvases.RemoveAt(lastCanvases.Count - 1);

            // Переключаемся без добавления текущего в историю (чтобы не зациклиться)
            if (currentActiveCanvas != null) currentActiveCanvas.SetActive(false);
            prev.SetActive(true);
            currentActiveCanvas = prev;
            ApplyAttributes(prev, true);
        }
    }
}