using UnityEngine;
using UnityEngine.UI;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class playerNameSaver : MonoBehaviour
{
    public playerDataClass playerData;
    public InputField nameInputField;
    public Button confirmButton;

    private string filePath;

    void Awake()
    {
#if UNITY_EDITOR
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        filePath = Path.Combine(scriptDirectory, "playerData.json");
#else
        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
#endif

        Debug.Log("Путь к файлу: " + filePath);
    }

    void Start()
    {
        if (nameInputField == null)
        {
            Debug.LogError("nameInputField не привязан в Inspector!");
            return;
        }

        if (confirmButton == null)
        {
            Debug.LogError("confirmButton не привязан в Inspector!");
            return;
        }

        // Подписываемся на изменение текста
        nameInputField.onValueChanged.AddListener(OnNameInputChanged);

        // Изначально кнопка отключена
        confirmButton.interactable = false;
    }

    // Вызывается при каждом изменении текста в InputField
    void OnNameInputChanged(string newText)
    {
        // Проверяем: длина больше 1 символа?
        confirmButton.interactable = newText.Length > 1;
    }

    public void SaveName()
    {
        // Дополнительная проверка (на случай, если кто-то вызовет метод вручную)
        if (string.IsNullOrEmpty(nameInputField.text) || nameInputField.text.Length <= 1)
        {
            Debug.LogWarning("Имя слишком короткое — сохранение отменено.");
            return;
        }

        playerData.data.player_name = nameInputField.text;
        Debug.Log("Имя сохранено в player_name: " + playerData.data.player_name);

        // Здесь можно добавить реальное сохранение в файл, если нужно:
        // SaveToFile();
    }

    // Пример метода сохранения в файл (опционально)
    /*
    void SaveToFile()
    {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(filePath, json);
        Debug.Log("Данные сохранены в: " + filePath);
    }
    */
}