using UnityEngine;
using UnityEngine.UI;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class playerNameSaver : MonoBehaviour
{
    playerData playerData;

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
        LoadData();
        playerData.player_name = nameInputField.text;
        SaveData();

        Debug.Log("Имя сохранено в player_name: " + playerData.player_name);
    }

    void SaveData()
    {
        try
        {
            // Сохраняем только имя в текстовый файл *** ***** *******
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            //creating directory 

            string rawJSON = JsonUtility.ToJson(playerData, true);
            //serializing

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(rawJSON);//magic to write to file
                }
            }
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
                string rawJSON;
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        rawJSON = reader.ReadToEnd();//magic to read from file
                    }
                }
                playerData = JsonUtility.FromJson<playerData>(rawJSON);

                Debug.Log("Имя загружено из файла: " + playerData.player_name);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при загрузке файла: " + e.Message);
            }
        }
        else
        {
            playerData = new();
        }
    }
}