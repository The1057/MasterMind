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
        // В редакторе Unity сохраняем файл рядом со скриптом
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        filePath = Path.Combine(scriptDirectory, "playerData.json");
#else
        // На Android сохраняем в Application.persistentDataPath
        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
#endif

        Debug.Log("Путь к файлу: " + filePath); // Логируем путь для проверки
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
    }

    public void SaveName()
    {
        playerData.data.player_name = nameInputField.text;
        Debug.Log("Имя сохранено в player_name: " + playerData.data.player_name);
    }
}