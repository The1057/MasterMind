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
        nameFilePath = Path.Combine(Application.persistentDataPath, "Player_Data.txt"); // ������ � .txt
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
                Debug.Log("��������� ���: " + playerName);
            }
            catch (System.Exception e)
            {
                Debug.LogError("������ ��� ������ �����: " + e.Message);
            }
        }
        else
        {
            Debug.LogError("���� �� ������: " + nameFilePath);
            playerName = "����������";
        }
    }

    void UpdateNameDisplay()
    {
        if (nameDisplay != null)
        {
            nameDisplay.text = "���: " + playerName;
        }
        else
        {
            Debug.LogError("������ TextMeshProUGUI ��� ����������� ����� �� �������� � Inspector!");
        }
    }
}