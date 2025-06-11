using NUnit.Framework;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class testScript : MonoBehaviour
{
    public TextMeshProUGUI TextMeshProUGUI;
    public string testScoreBuffer = "TestScoreBuffer.txt";
    public string jsonScoreBuffer = "TestScoreBuffer.json";
    public string nextQuestionScene = "Map";
    public Button nextQuestionButton;
    public Canvas canvas;

    public List<GameObject> questions;
    public int currentQuestion=0;

    public Score score1 = new Score();

    public List<bool> firstPresses = new List<bool>();
    public List<TextMeshProUGUI> explanations = new List<TextMeshProUGUI>();

    [Serializable]
    public class Score
    {
        public int score;
        public int errorCount;
    }

    void Start()
    {
        foreach(var q in questions)
        {
            firstPresses.Add(true);
            //explanations.Add(FindFirstObjectByType<TextMeshProUGUI>());
        }
    }
    public void correctOption()
    {
        TextMeshProUGUI.color = Color.black;

        string fullJsonPath = GetFullPath(jsonScoreBuffer);

        bool dataExists = File.Exists(fullJsonPath) && File.ReadAllText(fullJsonPath).Trim() != "";
        if (dataExists && firstPresses[currentQuestion])
        {
            string rawJson = File.ReadAllText(fullJsonPath);
            JsonUtility.FromJsonOverwrite(rawJson, score1);

            score1.score++;

            rawJson = JsonUtility.ToJson(score1);
            print(rawJson);
            File.WriteAllText(fullJsonPath, rawJson);

            firstPresses[currentQuestion] = false;
            nextQuestionButton.transform.position = getCanvasCoords(new Vector3(0, -1000, 0));
        }
        else if (!firstPresses[currentQuestion])
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
        if (firstPresses[currentQuestion] && dataExists)
        {
            string rawJson = File.ReadAllText(fullJsonPath);
            JsonUtility.FromJsonOverwrite(rawJson, score1);

            score1.errorCount++;

            rawJson = JsonUtility.ToJson(score1);
            print(rawJson);
            File.WriteAllText(fullJsonPath, rawJson);

            firstPresses[currentQuestion] = false;
            TextMeshProUGUI.color = Color.white;
        }
        else
        {
            //если не первый неправильный ответ
        }
    }


    public void correctOptionNoFile()
    {
        TextMeshProUGUI.color = Color.white;

        if (firstPresses[currentQuestion])
        {
            score1.score++;
            firstPresses[currentQuestion] = false;
        }
        else
        {
            //если не первый правильный ответ
        }
    }
    public void incorrectOptionNoFile()
    {
        if (firstPresses[currentQuestion])
        {
            score1.errorCount++;

            firstPresses[currentQuestion] = false;
            TextMeshProUGUI.color = Color.white;
        }
        else
        {
            //если не первый неправильный ответ
        }
    }

    public void nextQuestion()
    {
        questions[currentQuestion].SetActive(false);
        currentQuestion++;
        questions[currentQuestion].SetActive(true);        
        //здесь делай свою магию с переключением сцен        
    }
    public void setQuestion(int questionIndex)
    {
        questions[currentQuestion].SetActive(false);
        currentQuestion = questionIndex;
        questions[currentQuestion].SetActive(true);
    
        //TextMeshProUGUI = FindFirstObjectByType<TextMeshProUGUI>();
        TextMeshProUGUI = GameObject.Find("Пояснение").GetComponent<TextMeshProUGUI>();
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
    public void forceFirstPresses()
    {
        for (int i = 0; i < firstPresses.Count; i++)
        {
            firstPresses[i] = true;
        }
    }
    [ContextMenu("Hide Current Explanation")]
    public void hideExplanation()
    {
        TextMeshProUGUI.color = Color.clear;

    }
}
