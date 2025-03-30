using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Button), typeof(Image))]
public class ButtonAnimation : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string targetSceneName; // �������� ������� �����

    [Header("Sprite Settings")]
    [SerializeField] private Sprite defaultSprite;   // ������ �� ���������
    [SerializeField] private Sprite activeSprite;    // ������ ��� �������� �����

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.2f; // ����� ������������ ��������
    [SerializeField] private float scaleFactor = 0.9f;       // ����������� ���������������
    [SerializeField] private bool useColorAnimation = true;  // ������������ �������� �����
    [SerializeField] private float colorMultiplier = 0.8f;   // ��������� ����������

    private Button button;
    private Image buttonImage;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Color originalColor;
    private bool isAnimating = false;

    private void Awake()
    {
        // �������� ����������� ����������
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // ��������� �������� ���������
        originalScale = rectTransform.localScale;
        originalColor = buttonImage.color;

        // ������������� ��������� ������
        if (defaultSprite != null)
        {
            buttonImage.sprite = defaultSprite;
        }

        // ������������� �� ������� �������
        button.onClick.AddListener(OnButtonClick);

        // ��������� ��������
        ValidateSettings();
    }

    private void Start()
    {
        // ��������� ������� ����� � ������ ������ ���� �����
        if (SceneManager.GetActiveScene().name == targetSceneName && activeSprite != null)
        {
            buttonImage.sprite = activeSprite;
        }
    }

    private void OnButtonClick()
    {
        if (!isAnimating && !string.IsNullOrEmpty(targetSceneName))
        {
            StartCoroutine(ButtonPressAnimation());
        }
    }

    private IEnumerator ButtonPressAnimation()
    {
        isAnimating = true;
        float halfDuration = animationDuration / 2f;
        float elapsedTime = 0f;
        Vector3 targetScale = originalScale * scaleFactor;
        Color targetColor = useColorAnimation ? originalColor * colorMultiplier : originalColor;

        // �������� �������
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;

            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            if (useColorAnimation)
            {
                buttonImage.color = Color.Lerp(originalColor, targetColor, t);
            }
            yield return null;
        }

        // �������� ��������
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;

            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            if (useColorAnimation)
            {
                buttonImage.color = Color.Lerp(targetColor, originalColor, t);
            }
            yield return null;
        }

        // ��������������� �������� ���������
        rectTransform.localScale = originalScale;
        buttonImage.color = originalColor;

        // ������� �� ������� �����
        SceneManager.LoadScene(targetSceneName);

        isAnimating = false;
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("Target Scene Name �� ������ ��� ������: " + gameObject.name);
        }

        if (defaultSprite == null)
        {
            Debug.LogWarning("Default Sprite �� ������ ��� ������: " + gameObject.name);
        }

        if (activeSprite == null)
        {
            Debug.LogWarning("Active Sprite �� ������ ��� ������: " + gameObject.name);
        }

        animationDuration = Mathf.Max(0.1f, animationDuration);
        scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 1f);
        colorMultiplier = Mathf.Clamp01(colorMultiplier);
    }

    private void OnDestroy()
    {
        // ������� �������� �� �������
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // �������������� �������� ����������� � ���������
        if (!GetComponent<Button>())
        {
            gameObject.AddComponent<Button>();
        }
        if (!GetComponent<Image>())
        {
            gameObject.AddComponent<Image>();
        }
    }
#endif
}