using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class testScript : MonoBehaviour
{
    public TextMeshProUGUI TextMeshProUGUI;
    public string testScoreBuffer = "TestScoreBuffer.txt";
    public string jsonScoreBuffer = "TestScoreBuffer.json";
    public string nextQuestionScene = "Map";
    public Button nextQuestionButton;
    public Canvas canvas;

    Score score1 = new Score();

    bool firstPress = true;

    [Serializable]
    private class Score
    {
        public int score;
        public int errorCount;
    }

    //void Start()
    //{
    //    TextMeshProUGUI.color = new Color(0, 0, 0, 0);
    //}

    public void correctOption()
    {
        TextMeshProUGUI.color = Color.black;

        string fullJsonPath = GetFullPath(jsonScoreBuffer);

        bool dataExists = File.Exists(fullJsonPath) && File.ReadAllText(fullJsonPath).Trim() != "";
        if (dataExists && firstPress)
        {
            string rawJson = File.ReadAllText(fullJsonPath);
            JsonUtility.FromJsonOverwrite(rawJson, score1);

            score1.score++;

            rawJson = JsonUtility.ToJson(score1);
            print(rawJson);
            File.WriteAllText(fullJsonPath, rawJson);

            firstPress = false;
            nextQuestionButton.transform.position = getCanvasCoords(new Vector3(0, -1000, 0));
        }
        else if (!firstPress)
        {
            //если не первый правильный ответ
        }
        else
        {
            print("Data doesnt exist");
        }
    }
    public void incorrectOption()
    {
        string fullJsonPath = GetFullPath(jsonScoreBuffer);

        bool dataExists = File.Exists(fullJsonPath) && File.ReadAllText(fullJsonPath).Trim() != "";
        if (firstPress && dataExists)
        {
            string rawJson = File.ReadAllText(fullJsonPath);
            JsonUtility.FromJsonOverwrite(rawJson, score1);

            score1.errorCount++;

            rawJson = JsonUtility.ToJson(score1);
            print(rawJson);
            File.WriteAllText(fullJsonPath, rawJson);

            firstPress = false;
            TextMeshProUGUI.color = Color.black;
        }
        else
        {
            //если не первый неправильный ответ
        }
    }

    public void nextQuestion()
    {
        //здесь делай свою магию с переключением сцен        
    }


    private string GetFullPath(string fileName)
    {
#if UNITY_EDITOR
        string scriptPath = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        return Path.Combine(scriptDirectory, fileName);
#else
        return Path.Combine(Application.persistentDataPath, fileName);
#endif
    }
    private Vector3 getCanvasCoords(Vector3 absCoords)
    {
        Vector3 Coords = Vector3.zero;

        Coords += canvas.transform.position + absCoords;

        return Coords;
    }

    [ContextMenu("Force first press")]
    public void forceFirstPress()
    {
        firstPress = true;
    }
}
