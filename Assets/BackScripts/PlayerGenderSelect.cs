using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Analytics;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerGenderSelect : MonoBehaviour
{
    public playerData playerData;
    public Button maleButton;
    public Button femaleButton;

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

        LoadGender();
    }

    void Start()
    {
        if (maleButton == null)
        {
            Debug.LogError("Кнопка мужского пола не привязана в Inspector!");
            return;
        }

        if (femaleButton == null)
        {
            Debug.LogError("Кнопка женского пола не привязана в Inspector!");
            return;
        }
        LoadGender();
    }
    void SaveGender()
    {
        try
        {
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
            Debug.LogError("Ошибка при сохранении файла гендера: " + e.Message);
        }
    }

    void LoadGender()
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
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при загрузке файла гендера: " + e.Message);
            }
        }
    }

    public void selectMale()
    {
        playerData.player_gender = "M";
        SaveGender();
    }
    public void selectFemale()
    {
        playerData.player_gender = "F";
        SaveGender();
    }
}