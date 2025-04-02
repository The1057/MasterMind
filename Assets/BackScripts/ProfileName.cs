using UnityEngine;
using TMPro; 
using System.IO;

public class ProfileName : MonoBehaviour
{
    public TextMeshProUGUI nameDisplay; 
    private string playerName = "";
    private string nameFilePath;

    void Start()
    {
#if UNITY_EDITOR
        string scriptPath = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        nameFilePath = Path.Combine(scriptDirectory, "Player_Data.txt"); 
#else
        nameFilePath = Path.Combine(Application.persistentDataPath, "Player_Data.txt"); // Теперь с .txt
#endif

        LoadNameFromFile();
        UpdateNameDisplay();
    }

    void LoadNameFromFile()
    {
        if (File.Exists(nameFilePath))
        {
            try
            {
                playerName = File.ReadAllText(nameFilePath).Trim();
                Debug.Log("Считанное имя: " + playerName);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при чтении файла: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("Файл не найден: " + nameFilePath);
            playerName = "Неизвестно";
        }
    }

    void UpdateNameDisplay()
    {
        if (nameDisplay != null)
        {
            nameDisplay.text = "Имя: " + playerName;
        }
        else
        {
            Debug.LogError("Объект TextMeshProUGUI для отображения имени не привязан в Inspector!");
        }
    }
}