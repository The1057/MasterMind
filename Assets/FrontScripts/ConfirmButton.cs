using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ConfirmButton : MonoBehaviour
{
    [Header("Button Settings")]
    public Sprite pressedSprite; // ������ ����� �������
    public string nextSceneName = "SelectMW3"; // ��� ��������� �����
    public float delay = 0.5f; // �������� ����� ������� ��������

    [Header("Transition Settings")]
    public float fadeDuration = 0.7f; // ������������ ����������
    public AnimationCurve fadeCurve; // ������ ��� �������� ����������
    [SerializeField] private Image fadeOverlay; // UI Image ��� ����������

    [Header("Animation Settings")]
    public float scaleDuration = 0.1f; // ������������ �������� ��������

    private Image buttonImage;
    private Sprite originalSprite;
    private bool isTransitioning = false;

    void Start()
    {
        // �������� Image ������
        buttonImage = GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogError("Image ��������� �� ������ �� �������: " + gameObject.name);
        }
        else
        {
            originalSprite = buttonImage.sprite;
        }

        // ������� fade overlay, ���� �� �� ��������
        if (fadeOverlay == null)
        {
            fadeOverlay = CreateFadeOverlay();
        }
        fadeOverlay.color = Color.clear; // ���������� ����������

        // �������������� ������, ���� ��� �� ������
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

        // ������ ������ ������
        if (buttonImage != null && pressedSprite != null)
        {
            buttonImage.sprite = pressedSprite;
        }
        else
        {
            Debug.LogWarning("buttonImage ��� pressedSprite �� ������!");
        }

        // ��������� �������� �������� � �������
        StartCoroutine(ScaleButton());
        StartCoroutine(LoadNextScene());
    }

    IEnumerator ScaleButton()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 pressedScale = originalScale * 0.95f; // ��������� �������

        // ����������
        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, pressedScale, elapsed / scaleDuration);
            yield return null;
        }

        //  // ������� � ��������� ��������
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

        // ������� ���������� (������ -> ����� -> ������)
        float elapsedTime = 0f;
        Color startColor = Color.clear;
        Color midColor = Color.white;
        Color endColor = Color.black;

        // ������� ��������� �� ������
        while (elapsedTime < fadeDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = fadeCurve.Evaluate(elapsedTime / (fadeDuration / 2));
            fadeOverlay.color = Color.Lerp(startColor, midColor, t);
            yield return null;
        }

        // ����� ��������� �� �������
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