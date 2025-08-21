using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BusinessPanelController : MonoBehaviour
{
    [Header("UI ссылки")]
    public Button analyzeButton;          // Кнопка "Анализ бизнеса"
    public RectTransform panel;           // Панель, которая появляется
    public CanvasGroup panelCanvasGroup;  // Чтобы управлять прозрачностью
    public TextMeshProUGUI panelText;     // Текст внутри панели

    private bool isVisible = false;

    void Start()
    {
        // Скрываем панель в начале
        panel.localScale = Vector3.zero;
        panelCanvasGroup.alpha = 0;
        panel.gameObject.SetActive(false);

        // Вешаем обработчик на кнопку
        analyzeButton.onClick.AddListener(TogglePanel);
    }

    void TogglePanel()
    {
        if (!isVisible)
            StartCoroutine(ShowPanel());
        else
            StartCoroutine(HidePanel());
    }

    IEnumerator ShowPanel()
    {
        panel.gameObject.SetActive(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f; // скорость
            panel.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            panelCanvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }

        isVisible = true;
    }

    IEnumerator HidePanel()
    {
        float t = 0f;
        Vector3 startScale = panel.localScale;
        float startAlpha = panelCanvasGroup.alpha;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            panel.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t);
            yield return null;
        }

        panel.gameObject.SetActive(false);
        isVisible = false;
    }
}
