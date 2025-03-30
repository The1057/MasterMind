using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Button), typeof(Image))]
public class ButtonAnimation : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string targetSceneName; // Название целевой сцены

    [Header("Sprite Settings")]
    [SerializeField] private Sprite defaultSprite;   // Спрайт по умолчанию
    [SerializeField] private Sprite activeSprite;    // Спрайт для активной сцены

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.2f; // Общая длительность анимации
    [SerializeField] private float scaleFactor = 0.9f;       // Коэффициент масштабирования
    [SerializeField] private bool useColorAnimation = true;  // Использовать анимацию цвета
    [SerializeField] private float colorMultiplier = 0.8f;   // Множитель затемнения

    private Button button;
    private Image buttonImage;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Color originalColor;
    private bool isAnimating = false;

    private void Awake()
    {
        // Получаем необходимые компоненты
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // Сохраняем исходные параметры
        originalScale = rectTransform.localScale;
        originalColor = buttonImage.color;

        // Устанавливаем начальный спрайт
        if (defaultSprite != null)
        {
            buttonImage.sprite = defaultSprite;
        }

        // Подписываемся на событие нажатия
        button.onClick.AddListener(OnButtonClick);

        // Валидация настроек
        ValidateSettings();
    }

    private void Start()
    {
        // Проверяем текущую сцену и меняем спрайт если нужно
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

        // Анимация нажатия
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

        // Анимация возврата
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

        // Восстанавливаем исходные параметры
        rectTransform.localScale = originalScale;
        buttonImage.color = originalColor;

        // Переход на целевую сцену
        SceneManager.LoadScene(targetSceneName);

        isAnimating = false;
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogWarning("Target Scene Name не указан для кнопки: " + gameObject.name);
        }

        if (defaultSprite == null)
        {
            Debug.LogWarning("Default Sprite не указан для кнопки: " + gameObject.name);
        }

        if (activeSprite == null)
        {
            Debug.LogWarning("Active Sprite не указан для кнопки: " + gameObject.name);
        }

        animationDuration = Mathf.Max(0.1f, animationDuration);
        scaleFactor = Mathf.Clamp(scaleFactor, 0.1f, 1f);
        colorMultiplier = Mathf.Clamp01(colorMultiplier);
    }

    private void OnDestroy()
    {
        // Очищаем подписку на событие
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Автоматическая проверка компонентов в редакторе
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