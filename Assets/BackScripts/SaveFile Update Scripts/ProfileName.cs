using UnityEngine;
using TMPro; 
using System.IO;
using UnityEditor;

public class ProfileName : MonoBehaviour
{
    public TextMeshProUGUI nameDisplay; 
    private string playerName = "";
    private string filePath;

    void Start()
    {
#if UNITY_EDITOR
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        filePath = Path.Combine(scriptDirectory, "playerData.json");
#else
        filePath = Path.Combine(Application.persistentDataPath, "playerData.json");
#endif

        LoadNameFromFile();
        UpdateNameDisplay();
    }

    void LoadNameFromFile()
    {
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
                playerName = playerData.player_name;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при загрузке файла гендера: " + e.Message);
                playerName = "";
            }
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