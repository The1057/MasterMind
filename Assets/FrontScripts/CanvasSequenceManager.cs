using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasSequenceManager : MonoBehaviour
{
    [Header("Список Canvas (в порядке переключения)")]
    public GameObject[] canvases;

    [Header("Настройки анимации")]
    public float animationDuration = 0.5f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
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
            var go = canvases[i];
            go.SetActive(false);

            graphicsCache[i] = go.GetComponentsInChildren<Graphic>(true);
            tmpCache[i] = go.GetComponentsInChildren<TextMeshProUGUI>(true);
        }

        ShowNextCanvas();  // Показываем первый Canvas
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentIndex < canvases.Length - 1)
        {
            ShowNextCanvas();  // Показываем следующий Canvas только если это не последний
        }
    }

    private void ShowNextCanvas()
    {
        if (currentIndex >= 0)
            canvases[currentIndex].SetActive(false);

        currentIndex++;
        if (currentIndex >= canvases.Length) return; // Выход из функции, если мы дошли до последнего

        StartCoroutine(AnimateCanvas(currentIndex));
    }

    private IEnumerator AnimateCanvas(int index)
    {
        GameObject go = canvases[index];
        go.SetActive(true);

        go.transform.localScale = Vector3.zero;
        SetAlpha(index, 0f);

        float time = 0f;
        while (time < animationDuration)
        {
            float t = time / animationDuration;
            float s = scaleCurve.Evaluate(t);
            float a = alphaCurve.Evaluate(t);

            go.transform.localScale = Vector3.one * s;
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
