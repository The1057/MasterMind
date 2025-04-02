using UnityEngine;
using UnityEngine.UI;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player_Data : MonoBehaviour
{
    public string player_name = "";
    public string player_gender = "";
    public int first_niche;
    public int legal_form;
    public int tax_system;

    public InputField nameInputField;
    public Button confirmButton;

    private string filePath;

    void Awake()
    {
#if UNITY_EDITOR
        // В редакторе Unity сохраняем файл рядом со скриптом
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        filePath = Path.Combine(scriptDirectory, "Player_Data.txt");
#else
        // На Android сохраняем в Application.persistentDataPath
        filePath = Path.Combine(Application.persistentDataPath, "Player_Data.txt");
#endif

        Debug.Log("Путь к файлу: " + filePath); // Логируем путь для проверки
        LoadData();
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

        confirmButton.onClick.AddListener(SaveName);
    }

    void SaveName()
    {
        player_name = nameInputField.text;
        SaveData();

        Debug.Log("Имя сохранено в player_name: " + player_name);
    }

    void SaveData()
    {
        try
        {
            // Сохраняем только имя в текстовый файл
            File.WriteAllText(filePath, player_name);
            Debug.Log("Данные сохранены в файл: " + filePath);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка при сохранении файла: " + e.Message);
        }
    }

    void LoadData()
    {
        if (File.Exists(filePath))
        {
            try
            {
                player_name = File.ReadAllText(filePath);
                Debug.Log("Имя загружено из файла: " + player_name);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при загрузке файла: " + e.Message);
            }
        }
    }
}