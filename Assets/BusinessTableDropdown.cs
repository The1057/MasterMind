using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class BusinessTableAnimated : MonoBehaviour
{
    [Header("UI")]
    public RectTransform panel;             // Панель, внутри которой всё растягивается
    public Button analyzeButton;           // Кнопка-фон
    public Sprite pressedSprite;           // Спрайт при раскрытии
    public List<RectTransform> rows;       // Строки

    [Header("Layout")]
    public float rowHeight = 40f;
    public float spacing = 6f;
    public float paddingTop = 8f;
    public float paddingBottom = 8f;

    [Header("Animation")]
    public float animationDuration = 0.4f;
    public float delayBetween = 0.05f;

    [Header("Position")]
    public Vector2 startPosition = new Vector2(0f, -127f);

    private RectTransform buttonRect;
    private Image buttonImage;
    private Sprite originalSprite;
    private float headerHeight;
    private bool isOpen = false;

    void Awake()
    {
        if (analyzeButton == null || panel == null)
        {
            Debug.LogError("Analyze Button или Panel не установлены!");
            return;
        }

        buttonRect = analyzeButton.GetComponent<RectTransform>();
        buttonImage = analyzeButton.GetComponent<Image>();

        if (buttonImage != null)
            originalSprite = buttonImage.sprite;

        headerHeight = buttonRect.sizeDelta.y;

        // Ставим кнопку внутри панели
        buttonRect.SetParent(panel, false);
        buttonRect.anchoredPosition = startPosition;
        buttonRect.pivot = new Vector2(0.5f, 1f);
        buttonRect.anchorMin = new Vector2(buttonRect.anchorMin.x, 1f);
        buttonRect.anchorMax = new Vector2(buttonRect.anchorMax.x, 1f);

        // Подготовка строк внутри кнопки
        for (int i = 0; i < rows.Count; i++)
        {
            RectTransform row = rows[i];
            row.SetParent(buttonRect, false);
            row.anchorMin = new Vector2(0f, 1f);
            row.anchorMax = new Vector2(1f, 1f);
            row.pivot = new Vector2(0.5f, 1f);
            row.sizeDelta = new Vector2(0f, rowHeight);
            row.localScale = new Vector3(1f, 0f, 1f);
            row.gameObject.SetActive(false);
        }

        analyzeButton.onClick.AddListener(ToggleTable);
    }

    void ToggleTable()
    {
        StopAllCoroutines();

        if (!isOpen && pressedSprite != null)
            buttonImage.sprite = pressedSprite;

        if (isOpen) StartCoroutine(HideTable());
        else StartCoroutine(ShowTable());
    }

    IEnumerator ShowTable()
    {
        isOpen = true;

        float contentHeight = paddingTop + paddingBottom + rows.Count * rowHeight + (rows.Count - 1) * spacing;
        float targetHeight = headerHeight + contentHeight;

        for (int i = 0; i < rows.Count; i++)
        {
            rows[i].anchoredPosition = new Vector2(0f, -headerHeight - paddingTop - i * (rowHeight + spacing));
            rows[i].localScale = new Vector3(1f, 0f, 1f);
            rows[i].gameObject.SetActive(true);
        }

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < rows.Count; i++)
            {
                float rowT = Mathf.Clamp01((smoothT - i * delayBetween) / (1f - i * delayBetween));
                float rowSmooth = Mathf.SmoothStep(0f, 1f, rowT);
                rows[i].localScale = new Vector3(1f, rowSmooth, 1f);
            }

            buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, Mathf.Lerp(headerHeight, targetHeight, smoothT));
            yield return null;
        }

        foreach (var row in rows)
            row.localScale = Vector3.one;

        buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, targetHeight);
    }

    IEnumerator HideTable()
    {
        isOpen = false;

        float startHeight = buttonRect.sizeDelta.y;
        float targetHeight = headerHeight;

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            for (int i = 0; i < rows.Count; i++)
            {
                int idx = rows.Count - 1 - i;
                float rowT = Mathf.Clamp01((smoothT - i * delayBetween) / (1f - i * delayBetween));
                float rowSmooth = Mathf.SmoothStep(1f, 0f, rowT);
                rows[idx].localScale = new Vector3(1f, rowSmooth, 1f);
            }

            buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, Mathf.Lerp(startHeight, targetHeight, smoothT));
            yield return null;
        }

        foreach (var row in rows)
        {
            row.localScale = new Vector3(1f, 0f, 1f);
            row.gameObject.SetActive(false);
        }

        buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, targetHeight);

        if (buttonImage != null && originalSprite != null)
            buttonImage.sprite = originalSprite;
    }
}
