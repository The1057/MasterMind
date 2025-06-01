using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public class TapToProfile : MonoBehaviour
{
    public string maleProfileSceneName = "ProfileMan";
    public string femaleProfileSceneName = "ProfileW";
    private string filePath;
    private string playerGender = "";
    private bool genderLoaded = false;

    void Start()
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

    void Update()
    {
  
        if (genderLoaded && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            LoadProfileScene();
        }
    }

    void LoadGender()
    {
        genderLoaded = true;
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
                playerGender = playerData.player_gender;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при загрузке файла гендера: " + e.Message);
                playerGender = "";
            }
        }
    }

    void LoadProfileScene()
    {
        if (playerGender == "M")
        {
            SceneManager.LoadScene(maleProfileSceneName);
        }
        else if (playerGender == "F")
        {
            SceneManager.LoadScene(femaleProfileSceneName);
        }
        else if (genderLoaded) 
        {
            Debug.LogWarning("Неизвестное значение гендера: " + playerGender + ". Переход отменен.");
           
        }
        else
        {
            Debug.Log("Гендер еще не загружен. Действие отложено.");
        }
    }
}