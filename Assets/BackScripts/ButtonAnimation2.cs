using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using UnityEditor;

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

    private string filePath = "playerData.json"; // Имя файла с гендером

    private void Awake()
    {
#if UNITY_EDITOR
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        filePath = Path.Combine(scriptDirectory, "playerData.json");
#else
        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
#endif


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

        if (gender == "F")
        {
            Debug.Log($"Переход на сцену: {femaleProfileScene}");
            SceneManager.LoadScene(femaleProfileScene);
        }
        else if (gender == "M")
        {
            Debug.Log($"Переход на сцену: {maleProfileScene}");
            SceneManager.LoadScene(maleProfileScene);
        }
        else
        {
            Debug.LogError($"Некорректное значение пола: '{gender}'. Ожидается 'F' или 'M'. Переход не выполнен.");
        }

        isAnimating = false;
    }

    private string GetPlayerGender()
    {
        //string fePath = GetFullPath(filePath);
        playerData playerData;
        if (File.Exists(filePath))
        {
            try
            {
                string rawJSON;
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        rawJSON = reader.ReadToEnd();//magic to read from file
                    }
                }
                playerData = JsonUtility.FromJson<playerData>(rawJSON);
                return playerData.player_gender;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при загрузке файла гендера: " + e.Message);
                return "F";
            }
        }else
        {
            return "F";
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