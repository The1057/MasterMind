using UnityEngine;
using UnityEngine.UI;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player_Data2 : MonoBehaviour
{
    public string player_gender = "";
    public Button maleButton;
    public Button femaleButton;

    private string filePath;

    void Awake()
    {
#if UNITY_EDITOR
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        filePath = Path.Combine(scriptDirectory, "player_gender.txt");
#else
        filePath = Path.Combine(Application.persistentDataPath, "player_gender.txt");
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

        maleButton.onClick.AddListener(SetMaleGender);
        femaleButton.onClick.AddListener(SetFemaleGender);
    }

    public void SetMaleGender()
    {
        player_gender = "М";
        SaveGender();
    }

    public void SetFemaleGender()
    {
        player_gender = "Ж";
        SaveGender();
    }

    void SaveGender()
    {
        try
        {
            File.WriteAllText(filePath, player_gender);
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
                player_gender = File.ReadAllText(filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при загрузке файла гендера: " + e.Message);
            }
        }
    }


}