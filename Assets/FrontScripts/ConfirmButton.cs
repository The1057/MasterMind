using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ConfirmButton : MonoBehaviour
{
    [Header("Button Settings")]
    public Sprite pressedSprite; // Спрайт после нажатия
    public string nextSceneName = "SelectMW3"; // Имя следующей сцены
    public float delay = 0.5f; // Задержка перед началом перехода

    [Header("Transition Settings")]
    public float fadeDuration = 0.7f; // Длительность затемнения
    public AnimationCurve fadeCurve; // Кривая для плавного затемнения
    [SerializeField] private Image fadeOverlay; // UI Image для затемнения

    [Header("Animation Settings")]
    public float scaleDuration = 0.1f; // Длительность анимации масштаба

    private Image buttonImage;
    private Sprite originalSprite;
    private bool isTransitioning = false;

    void Start()
    {
        // Получаем Image кнопки
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("Image компонент не найден на объекте: " + gameObject.name);
        }
        else
        {
            originalSprite = buttonImage.sprite;
        }

        // Создаем fade overlay, если он не назначен
        if (fadeOverlay == null)
        {
            fadeOverlay = CreateFadeOverlay();
        }
        fadeOverlay.color = Color.clear; // Изначально прозрачный

        // Инициализируем кривую, если она не задана
        if (fadeCurve == null || fadeCurve.length == 0)
        {
            fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }
    }

    private Image CreateFadeOverlay()
    {
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        GameObject fadeObj = new GameObject("FadeImage");
        fadeObj.transform.SetParent(canvasObj.transform, false);
        Image fadeImg = fadeObj.AddComponent<Image>();

        RectTransform rect = fadeImg.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        return fadeImg;
    }

    public void OnButtonClick()
    {
        if (isTransitioning) return;

        isTransitioning = true;

        // Меняем спрайт кнопки
        if (buttonImage != null && pressedSprite != null)
        {
            buttonImage.sprite = pressedSprite;
        }
        else
        {
            Debug.LogWarning("buttonImage или pressedSprite не заданы!");
        }

        // Запускаем анимацию масштаба и переход
        StartCoroutine(ScaleButton());
        StartCoroutine(LoadNextScene());
    }

    IEnumerator ScaleButton()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 pressedScale = originalScale * 0.95f; // Уменьшаем масштаб

        // Уменьшение
        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, pressedScale, elapsed / scaleDuration);
            yield return null;
        }

        //  // Возврат к исходному масштабу
        elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(pressedScale, originalScale, elapsed / scaleDuration);
            yield return null;
        }
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delay);

        // Плавное затемнение (черный -> белый -> черный)
        float elapsedTime = 0f;
        Color startColor = Color.clear;
        Color midColor = Color.white;
        Color endColor = Color.black;

        // Сначала затемняем до белого
        while (elapsedTime < fadeDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = fadeCurve.Evaluate(elapsedTime / (fadeDuration / 2));
            fadeOverlay.color = Color.Lerp(startColor, midColor, t);
            yield return null;
        }

        // Затем затемняем до черного
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = fadeCurve.Evaluate(elapsedTime / (fadeDuration / 2));
            fadeOverlay.color = Color.Lerp(midColor, endColor, t);
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}