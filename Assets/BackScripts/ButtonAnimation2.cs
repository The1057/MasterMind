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
    [SerializeField] private string femaleProfileScene = "ProfileW"; // ����� ��� �������� �������
    [SerializeField] private string maleProfileScene = "ProfileMan"; // ����� ��� �������� �������

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

    private string filePath = "playerData.json"; // ��� ����� � ��������

    private void Awake()
    {
#if UNITY_EDITOR
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        filePath = Path.Combine(scriptDirectory, "playerData.json");
#else
        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
#endif


        // �������� ����������
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        // ��������� �������� ��������� ��� ��������
        originalScale = rectTransform.localScale;
        originalColor = buttonImage.color;

        // ������������� �� ������� ������� ������
        button.onClick.AddListener(OnButtonClick);

        // ��������� ���������
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

        // ������ ��� � ��������� �� ������ �����
        string gender = GetPlayerGender();
        Debug.Log($"��������� ���: '{gender}' (����� ������: {gender.Length})");

        if (gender == "F")
        {
            Debug.Log($"������� �� �����: {femaleProfileScene}");
            SceneManager.LoadScene(femaleProfileScene);
        }
        else if (gender == "M")
        {
            Debug.Log($"������� �� �����: {maleProfileScene}");
            SceneManager.LoadScene(maleProfileScene);
        }
        else
        {
            Debug.LogError($"������������ �������� ����: '{gender}'. ��������� 'F' ��� 'M'. ������� �� ��������.");
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
                Debug.LogError("������ ��� �������� ����� �������: " + e.Message);
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
            Debug.LogWarning($"Female Profile Scene �� ������ ��� ������: {gameObject.name}");
        }

        if (string.IsNullOrEmpty(maleProfileScene))
        {
            Debug.LogWarning($"Male Profile Scene �� ������ ��� ������: {gameObject.name}");
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