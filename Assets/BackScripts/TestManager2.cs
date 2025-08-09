using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;

[Serializable]
public class Question
{
    public string question;
    public List<string> answers;
    public string comment;
    public char correctAnswerLetter;
}
[Serializable]
public class Score
{
    public int score;
    public int errorCount;
}

public class TestManager2 : MonoBehaviour
{
    public List<Question> questions;
    public List<GameObject> questionObjects = new();


    [Header("Префабы")]
    public GameObject QuestionPrefab;
    public GameObject answerButtPrefabIncorr;
    public GameObject answerButtPrefabCorr;
    public GameObject scrollPrefab;
    public GameObject qButtonPrefab;


    [Header("Кнопки вопросов")]

    public Sprite qButtPressed;
    public Sprite qButtDefault;
    public Sprite qButtComplete;


    [Header("Кнопки ответов")]

    public Sprite ansButtPressedCorrect;
    public Sprite ansButtPressedIncorrect;
    public Sprite ansButtDefault;

    [Header("Настройка текста")]

    public float ansButtonDistance = 150;
    public float qButtonDistance = 80;
    public int ansButt2QuestDistance = 20;
    public int ansButtSizeModifier = 20;
    public float comment2LastAnsGap = 100;

    [Header("Разное")]
    public TextMeshProUGUI TextMeshProUGUI;
    public GameObject scrollBar;
    public List<GameObject> qButtons = new();
    public int currentQuestion = 0;
    public List<bool> firstPresses = new List<bool>();
    public List<Button> correctButtons;
    public Score score1 = new Score();

    void Start()
    {
        generateTest();

    }

    void Update()
    {
        
    }

    public void correctOption()
    {
        if (firstPresses[currentQuestion])
        {
            var thisButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            thisButton.GetComponent<Image>().sprite = ansButtPressedCorrect;
        }

        if (firstPresses[currentQuestion])
        {
            score1.score++;
            firstPresses[currentQuestion] = false;
            TextMeshProUGUI.color = Color.white;
        }
        else
        {
            //если не первый правильный ответ
        }

    }

    public void incorrectOption()
    {
        if (firstPresses[currentQuestion])
        {
            var thisButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            thisButton.GetComponent<Image>().sprite = ansButtPressedIncorrect;
            correctButtons[currentQuestion].image.sprite = ansButtPressedCorrect;
        }

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

    public void setQuestion(GameObject button)
    {
        questionObjects[currentQuestion].SetActive(false);
        currentQuestion = int.Parse(button.GetComponentInChildren<TextMeshProUGUI>().text)-1;
        questionObjects[currentQuestion].SetActive(true);
        TextMeshProUGUI = GameObject.Find("Пояснение").GetComponent<TextMeshProUGUI>();

        var images = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<Image>();        
        for (int i = 0; i < images.Length; i++)
        {
            if (i == currentQuestion)
            {
                images[i].sprite = qButtPressed;
                this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
            else if(!firstPresses[i])
            {
                images[i].sprite = qButtComplete;
                this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                images[i].sprite = qButtDefault;
                this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i + 1).GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
        }


    }
    public List<Button> findAllCorrectAnswers()
    {
        var buttons = this.gameObject.transform.GetComponentsInChildren<Button>(true);
        List<Button> correctButtons = new();

        foreach (var button in buttons)
        {
            if (button.onClick.GetPersistentMethodName(0) == "correct")
            {
                //print(button.name + button.onClick.GetPersistentMethodName(0));
                correctButtons.Add(button);
            }
            //print(button.name + button.onClick.GetPersistentMethodName(0));
        }
        return correctButtons;
    }

    public void generateTest()
    {
        foreach (Question question in questions)
        {
            questionObjects.Add(Instantiate(QuestionPrefab, this.transform));

            TextMeshProUGUI = GameObject.Find("Вопрос").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI.text = question.question;

            GameObject lastButton = new();
            for (int i = 0; i < question.answers.Count; i++)
            {
                var answer = question.answers[i];
                float buttHeightModif = TextMeshProUGUI.preferredHeight + ansButt2QuestDistance; //(question.question.Length/18)*ansButt2QuestDistance;
                GameObject thisButton;
                if (answer[0] == question.correctAnswerLetter)
                {
                    thisButton = Instantiate(answerButtPrefabCorr, questionObjects.Last().transform);
                }
                else
                {
                    thisButton = Instantiate(answerButtPrefabIncorr, questionObjects.Last().transform);
                }
                thisButton.GetComponent<testAnsButtScript>().testManager = this;
                thisButton.GetComponentInChildren<TextMeshProUGUI>().text = answer;

                var buttSizeModifHeight = thisButton.GetComponentInChildren<TextMeshProUGUI>().preferredHeight;
                var buttSizeModifWidth = thisButton.GetComponent<RectTransform>().sizeDelta.x;
                thisButton.GetComponent<RectTransform>().sizeDelta = new Vector2(buttSizeModifWidth,buttSizeModifHeight + ansButtSizeModifier);
                var buttSize = thisButton.GetComponent<RectTransform>().sizeDelta;

                thisButton.transform.position -= new Vector3(0,buttSize.y/2 + buttHeightModif + ansButtonDistance * i, 0);
                lastButton = thisButton;
            }

            TextMeshProUGUI = GameObject.Find("Пояснение").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI.text = question.comment;
            TextMeshProUGUI.color = Color.clear;
            TextMeshProUGUI.gameObject.transform.position = lastButton.gameObject.transform.position 
                - new Vector3(0,comment2LastAnsGap + lastButton.GetComponent<RectTransform>().sizeDelta.y,0);

            


            firstPresses.Add(true);
            questionObjects.Last().gameObject.SetActive(false);
        }

        scrollBar = GameObject.Find("TestScrollView");
        for (int i = 0; i < questions.Count; i++)
        {
            var thisButton = Instantiate(scrollBar.transform.GetChild(0).GetChild(0).GetChild(0), scrollBar.transform.GetChild(0).GetChild(0));
            thisButton.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
        }       //создаём и расставляем элементы скролл бара

        scrollBar.transform.GetChild(0).GetChild(0).GetComponent<HorizontalLayoutGroup>().spacing = qButtonDistance;
        scrollBar.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);

        setQuestion(this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject);
        //выставляем первый вопрос

        correctButtons = findAllCorrectAnswers();

    }

}
