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
        // � ��������� Unity ��������� ���� ����� �� ��������
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        filePath = Path.Combine(scriptDirectory, "Player_Data.txt");
#else
        // �� Android ��������� � Application.persistentDataPath
        filePath = Path.Combine(Application.persistentDataPath, "Player_Data.txt");
#endif

        Debug.Log("���� � �����: " + filePath); // �������� ���� ��� ��������
        LoadData();
    }

    void Start()
    {
        if (nameInputField == null)
        {
            Debug.LogError("nameInputField �� �������� � Inspector!");
            return;
        }

        if (confirmButton == null)
        {
            Debug.LogError("confirmButton �� �������� � Inspector!");
            return;
        }

        confirmButton.onClick.AddListener(SaveName);
    }

    void SaveName()
    {
        player_name = nameInputField.text;
        SaveData();

        Debug.Log("��� ��������� � player_name: " + player_name);
    }

    void SaveData()
    {
        try
        {
            // ��������� ������ ��� � ��������� ����
            File.WriteAllText(filePath, player_name);
            Debug.Log("������ ��������� � ����: " + filePath);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError("������ ��� ���������� �����: " + e.Message);
        }
    }

    void LoadData()
    {
        if (File.Exists(filePath))
        {
            try
            {
                player_name = File.ReadAllText(filePath);
                Debug.Log("��� ��������� �� �����: " + player_name);
            }
            catch (System.Exception e)
            {
                Debug.LogError("������ ��� �������� �����: " + e.Message);
            }
        }
    }
}