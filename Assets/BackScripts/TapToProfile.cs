using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class TapToProfile : MonoBehaviour
{
    public string maleProfileSceneName = "ProfileMan";
    public string femaleProfileSceneName = "ProfileW";
    private string genderFilePath;
    private string playerGender = "";
    private bool genderLoaded = false;

    void Start()
    {
#if UNITY_EDITOR
        string scriptPath = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        genderFilePath = Path.Combine(scriptDirectory, "player_gender.txt");
#else
        genderFilePath = Path.Combine(Application.persistentDataPath, "player_gender.txt");
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
        if (File.Exists(genderFilePath))
        {
            try
            {
                playerGender = File.ReadAllText(genderFilePath).Trim().ToUpper();
                Debug.Log("����������� ������: " + playerGender);
                genderLoaded = true; 
            }
            catch (System.Exception e)
            {
                Debug.LogError("������ ��� ������ ����� �������: " + e.Message);
            }
        }
        else
        {
            Debug.Log("���� ������� �� ������. �������� ����� ��������.");
          
        }
    }

    void LoadProfileScene()
    {
        if (playerGender == "�")
        {
            SceneManager.LoadScene(maleProfileSceneName);
        }
        else if (playerGender == "�")
        {
            SceneManager.LoadScene(femaleProfileSceneName);
        }
        else if (genderLoaded) 
        {
            Debug.LogWarning("����������� �������� �������: " + playerGender + ". ������� �������.");
           
        }
        else
        {
            Debug.Log("������ ��� �� ��������. �������� ��������.");
        }
    }
}