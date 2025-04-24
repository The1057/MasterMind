using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

[RequireComponent(typeof(Button), typeof(Image))]
public class ButtonAnimation2 : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string femaleProfileScene = "ProfileW"; // Сцена для женского профиля
    [SerializeField] private string maleProfileScene = "ProfileMan"; // Сцена для мужского профиля

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

    private string genderFilePath = "player_gender.txt"; // Имя файла с гендером

    private void Awake()
    {
        // Получаем компоненты
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // Сохраняем исходные параметры для анимации
        originalScale = rectTransform.localScale;
        originalColor = buttonImage.color;

        // Подписываемся на событие нажатия кнопки
        button.onClick.AddListener(OnButtonClick);

        // Проверяем настройки
        ValidateSettings();
    }

    private void OnButtonClick()
    {
        if (!isAnimating)
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

        // Читаем пол и переходим на нужную сцену
        string gender = GetPlayerGender();
        Debug.Log($"Считанный пол: '{gender}' (длина строки: {gender.Length})");

        if (gender == "Ж")
        {
            Debug.Log($"Переход на сцену: {femaleProfileScene}");
            SceneManager.LoadScene(femaleProfileScene);
        }
        else if (gender == "М")
        {
            Debug.Log($"Переход на сцену: {maleProfileScene}");
            SceneManager.LoadScene(maleProfileScene);
        }
        else
        {
            Debug.LogError($"Некорректное значение пола: '{gender}'. Ожидается 'Ж' или 'М'. Переход не выполнен.");
        }

        isAnimating = false;
    }

    private string GetPlayerGender()
    {
        string fullPath = GetFullPath(genderFilePath);
        try
        {
            if (File.Exists(fullPath) && File.ReadAllText(fullPath).Trim() != "")
            {
                string gender = File.ReadAllText(fullPath).Trim().ToUpper();
                Debug.Log($"Файл найден: {fullPath}. Содержимое: '{gender}' (длина строки: {gender.Length})");

                // Проверяем, является ли содержимое "Ж" или "М"
                if (gender == "Ж")
                {
                    return "Ж";
                }
                else if (gender == "М")
                {
                    return "М";
                }
                else
                {
                    Debug.LogError($"Некорректное значение в файле {fullPath}: '{gender}'. Ожидается 'Ж' или 'М'.");
                    return "";
                }
            }
            else
            {
                Debug.LogError($"Файл не найден или пуст: {fullPath}");
                return "";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при чтении файла {fullPath}: {e.Message}");
            return "";
        }
    }

    private string GetFullPath(string fileName)
    {
#if UNITY_EDITOR
        string scriptPath = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        return Path.Combine(scriptDirectory, fileName);
#else
        return Path.Combine(Application.persistentDataPath, fileName);
#endif
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrEmpty(femaleProfileScene))
        {
            Debug.LogWarning($"Female Profile Scene не указан для кнопки: {gameObject.name}");
        }

        if (string.IsNullOrEmpty(maleProfileScene))
        {
            Debug.LogWarning($"Male Profile Scene не указан для кнопки: {gameObject.name}");
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