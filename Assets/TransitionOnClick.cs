using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TransitionOnClick : MonoBehaviour
{
    [Header("Что анимировать")]
    public GameObject targetCanvas;        // Canvas, который должен появиться
    public float moveDistance = 50f;       // На сколько уходит кнопка
    public float duration = 0.8f;          // Длительность анимации

    private Button button;
    private CanvasRenderer buttonRenderer;
    private CanvasRenderer targetRenderer;

    void Start()
    {
        button = GetComponent<Button>();
        buttonRenderer = GetComponent<CanvasRenderer>();
        if (targetCanvas != null)
            targetRenderer = targetCanvas.GetComponent<CanvasRenderer>();
        else
            Debug.LogError("Target Canvas не назначен!");
    }

    // Этот метод вешается на OnClick кнопки
    public void StartTransition()
    {
        StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        Vector3 buttonStartPos = transform.position;
        Color buttonStartColor = buttonRenderer.GetColor();
        Color targetStartColor = targetRenderer != null ? targetRenderer.GetColor() : Color.clear;

        // Убедимся, что целевой Canvas изначально невидим
        if (targetRenderer != null)
        {
            Color invisible = targetStartColor;
            invisible.a = 0f;
            targetRenderer.SetColor(invisible);
            targetCanvas.SetActive(true); // включаем, но он прозрачный
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Кнопка: уходит вверх и исчезает
            transform.position = Vector3.Lerp(buttonStartPos, buttonStartPos + Vector3.up * moveDistance, t);
            Color btnColor = buttonStartColor;
            btnColor.a = Mathf.Lerp(1f, 0f, t);
            buttonRenderer.SetColor(btnColor);

            // Целевой Canvas: появляется
            if (targetRenderer != null)
            {
                Color targetColor = targetStartColor;
                targetColor.a = Mathf.Lerp(0f, 1f, t);
                targetRenderer.SetColor(targetColor);
            }

            yield return null;
        }

        // Финал
        gameObject.SetActive(false); // скрываем кнопку
        // targetCanvas остаётся видимым
    }
}