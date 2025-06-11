using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Простой скрипт для плавного появления UI-панели (только fade alpha), без масштабирования и движения.
/// При активации объекта плавно изменяет прозрачность всех Image, Text, TextMeshPro, Button внутри.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UISimpleFadeIn : MonoBehaviour
{
    [Header("Настройки плавного появления")]
    [Tooltip("Длительность fade-in (сек)")]
    public float duration = 0.5f;

    [Tooltip("Кривая прозрачности (0→1)")]
    public AnimationCurve alphaCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private MaskableGraphic[] graphics;

    void Awake()
    {
        // Собираем все UI-элементы, поддерживающие alpha
        graphics = GetComponentsInChildren<MaskableGraphic>(true);
        // Сразу делаем их невидимыми
        SetAlpha(0f);
    }

    void OnEnable()
    {
        // Запускаем появление при каждом включении
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            float a = alphaCurve.Evaluate(t);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(1f);
    }

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
