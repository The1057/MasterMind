using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasSequenceManager : MonoBehaviour
{
    [System.Serializable]
    public class CanvasElement2
    {
        public GameObject canvas;
        public Button nextButton;
    }

    [Header("Список Canvas (в порядке переключения)")]
    public CanvasElement2[] canvases;

    [Header("Настройки анимации")]
    public float animationDuration = 0.5f;
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private int currentIndex = -1;

    private Graphic[][] graphicsCache;
    private TextMeshProUGUI[][] tmpCache;

    void Start()
    {
        int n = canvases.Length;
        graphicsCache = new Graphic[n][];
        tmpCache = new TextMeshProUGUI[n][];

        for (int i = 0; i < n; i++)
        {
            var el = canvases[i];

            if (el.canvas != null)
            {
                el.canvas.SetActive(false);
                graphicsCache[i] = el.canvas.GetComponentsInChildren<Graphic>(true);
                tmpCache[i] = el.canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
            }
            else
            {
                Debug.LogWarning($"Canvas is not assigned in element #{i} on '{gameObject.name}'", gameObject);
                graphicsCache[i] = new Graphic[0];
                tmpCache[i] = new TextMeshProUGUI[0];
            }

            // Подключаем кнопки
            if (el.nextButton != null)
                el.nextButton.onClick.AddListener(ShowNextCanvas);

        }
        ShowNextCanvas();  // Показываем первый Canvas
    }


    private void ShowNextCanvas()
    {
        if (currentIndex >= 0)
            canvases[currentIndex].canvas.SetActive(false);

        currentIndex++;
        if (currentIndex >= canvases.Length) return; // Выход из функции, если мы дошли до последнего

        StartCoroutine(AnimateCanvas(currentIndex));
    }

    private IEnumerator AnimateCanvas(int index)
    {
        GameObject go = canvases[index].canvas;
        go.SetActive(true);

        go.transform.localScale = Vector3.zero;
        SetAlpha(index, 0f);

        float time = 0f;
        while (time < animationDuration)
        {
            float t = time / animationDuration;
            float a = alphaCurve.Evaluate(t);

            SetAlpha(index, a);

            time += Time.deltaTime;
            yield return null;
        }

        go.transform.localScale = Vector3.one;
        SetAlpha(index, 1f);
    }

    private void SetAlpha(int idx, float alpha)
    {
        foreach (var g in graphicsCache[idx])
        {
            Color c = g.color;
            c.a = alpha;
            g.color = c;
        }
        foreach (var t in tmpCache[idx])
        {
            Color c = t.color;
            c.a = alpha;
            t.color = c;
        }
    }
}
