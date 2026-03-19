using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasSequenceManager23 : MonoBehaviour
{
    [System.Serializable]
    public class CanvasElement
    {
        [Tooltip("Сам Canvas, который будем показывать/прятать")]
        public GameObject canvas;
        [Tooltip("Кнопка перехода вперёд")]
        public Button nextButton;
        [Tooltip("Кнопка перехода назад")]
        public Button previousButton;
        [Tooltip("Кнопка выхода (если есть — сбросит теорию)")]
        public Button exitButton;
        public Button exitButton2;
    }

    [Header("Список Canvas-элементов")]
    public CanvasElement[] elements;

    [Header("Настройки анимации")]
    public float animationDuration = 0.5f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private int currentIndex = -1;
    int prevIndex;
    private Graphic[][] graphicsCache;
    private TextMeshProUGUI[][] tmpCache;

    void Start()
    {
        if (elements.Length == 0)
        {
            Debug.LogError("⚠️ Elements list is empty. Please assign canvases in the inspector.");
            return;
        }

        int n = elements.Length;
        graphicsCache = new Graphic[n][];
        tmpCache = new TextMeshProUGUI[n][];

        for (int i = 0; i < n; i++)
        {
            var el = elements[i];

            if (el.canvas != null)
            {
                el.canvas.SetActive(false);
                graphicsCache[i] = el.canvas.GetComponentsInChildren<Graphic>(true);
                tmpCache[i] = el.canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
            }
            else
            {
                Debug.LogWarning($"⚠️ Canvas is not assigned in element #{i} on '{gameObject.name}'", gameObject);
                graphicsCache[i] = new Graphic[0];
                tmpCache[i] = new TextMeshProUGUI[0];
            }

            // Подключаем кнопки
            if (el.nextButton != null)
                el.nextButton.onClick.AddListener(ShowNextCanvas);
            if (el.previousButton != null)
                el.previousButton.onClick.AddListener(ShowPreviousCanvas);
            if (el.exitButton != null) {
                el.exitButton.onClick.AddListener(ResetAndShowFirst);
                if (el.exitButton2 != null)
                    el.exitButton2.onClick.AddListener(ResetAndShowFirst);
            }
                
        }

        ShowNextCanvas(); // Показываем первый
    }

    private void ShowNextCanvas()
    {
        if (currentIndex >= elements.Length - 1) return;

        if (currentIndex >= 0 && elements[currentIndex].canvas != null)
            elements[currentIndex].canvas.SetActive(false);

        prevIndex = currentIndex;
        currentIndex++;
        StartCoroutine(altAnimateCanvas(currentIndex));
        UpdateButtonsState();
    }

    public void ShowPreviousCanvas()
    {
        if (currentIndex <= 0) return;

        if (elements[currentIndex].canvas != null)
            elements[currentIndex].canvas.SetActive(false);

        prevIndex = currentIndex;
        currentIndex--;
        StartCoroutine(altAnimateCanvas(currentIndex));
        UpdateButtonsState();
    }

    private void UpdateButtonsState()
    {
        for (int i = 0; i < elements.Length; i++)
        {
            bool isActiveElement = (i == currentIndex);
            var el = elements[i];

            if (el.nextButton != null)
                el.nextButton.interactable = isActiveElement && (currentIndex < elements.Length - 1);

            if (el.previousButton != null)
                el.previousButton.interactable = isActiveElement && (currentIndex > 0);
        }
    }

    private IEnumerator AnimateCanvas(int idx)
    {
        var el = elements[idx];

        if (el.canvas == null)
        {
            Debug.LogWarning($"⚠️ Canvas at index {idx} is null in '{gameObject.name}'", gameObject);
            yield break;
        }

        var go = el.canvas;
        go.SetActive(true);
        go.transform.localScale = Vector3.zero;
        SetAlpha(idx, 0f);

        float t = 0f;
        while (t < animationDuration)
        {
            float p = t / animationDuration;
            go.transform.localScale = Vector3.one * scaleCurve.Evaluate(p);
            SetAlpha(idx, alphaCurve.Evaluate(p));
            t += Time.deltaTime;
            yield return null;
        }

        go.transform.localScale = Vector3.one;
        SetAlpha(idx, 1f);
    }

    IEnumerator altAnimateCanvas(int idx)
    {
        var ghost = Instantiate(transform.parent.GetChild(idx).gameObject,transform.parent);
        print($"Instantiated{ghost}");
        ghost.transform.localPosition = Vector3.zero;
        ghost.transform.localScale = Vector3.one;
        ghost.SetActive(true );
        if (currentIndex < prevIndex)
        {
            ghost.transform.position -= Vector3.right * 1000;            
        }
        else
        {
            ghost.transform.position += Vector3.right * 1000;            
        }

        float t = 0;
        while (t < animationDuration)
        {
            float p = t / animationDuration;
            ghost.transform.localPosition = Vector3.Lerp(ghost.transform.localPosition, Vector3.zero, p);
            t += Time.deltaTime;
            yield return null;
        }
        ghost.SetActive(false);
        transform.parent.GetChild(idx).gameObject.SetActive(true);
        transform.parent.GetChild(idx).localScale = Vector3.one;
        Destroy(ghost);
    }

    private void SetAlpha(int idx, float alpha)
    {
        foreach (var g in graphicsCache[idx])
        {
            if (g != null)
            {
                Color c = g.color;
                c.a = alpha;
                g.color = c;
            }
        }

        foreach (var t in tmpCache[idx])
        {
            if (t != null)
            {
                Color c = t.color;
                c.a = alpha;
                t.color = c;
            }
        }
    }
    public void ResetAndShowFirst()
    {
        if (currentIndex >= 0 && currentIndex < elements.Length && elements[currentIndex].canvas != null)
        {
            elements[currentIndex].canvas.SetActive(false);
        }

        currentIndex = -1;

        ShowNextCanvas();

        Debug.Log("Теория сброшена. Показан первый слайд.");
    }
}
