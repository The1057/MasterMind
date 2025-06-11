using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Эффект появления: масштаб + fade всех UI-элементов (Image, Text, TextMeshPro, Button) внутри корневого объекта.
/// При активации GameObject будет проигрываться анимация.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UIFadeInOnEnable : MonoBehaviour
{
    [Header("Настройки появления")]
    [Tooltip("Длительность анимации (сек)")]
    public float duration = 0.5f;

    [Tooltip("Кривая масштабирования (0→1)")]
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Кривая прозрачности (0→1)")]
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private RectTransform rect;
    private MaskableGraphic[] graphics;

    void Awake()
    {
        // Кешируем компоненты
        rect = GetComponent<RectTransform>();
        // Собираем все графические компоненты (Image, Text, TMP, Button's targetGraphic и т.д.)
        graphics = GetComponentsInChildren<MaskableGraphic>(true);

        // Изначально скрываем объект
        rect.localScale = Vector3.zero;
        SetAlpha(0f);
    }

    void OnEnable()
    {
        // При каждом включении запускаем анимацию
        StartCoroutine(AnimateIn());
    }

    private IEnumerator AnimateIn()
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // масштаб
            float s = scaleCurve.Evaluate(t);
            rect.localScale = Vector3.one * s;

            // прозрачность
            float a = alphaCurve.Evaluate(t);
            SetAlpha(a);

            yield return null;
        }

        // финальные значения
        rect.localScale = Vector3.one;
        SetAlpha(1f);
    }

    // Устанавливает alpha у всех MaskableGraphic (Image, Text, TMP и т.п.)
    private void SetAlpha(float alpha)
    {
        foreach (var g in graphics)
        {
            if (g != null)
            {
                Color c = g.color;
                c.a = alpha;
                g.color = c;
            }
        }
    }
}
