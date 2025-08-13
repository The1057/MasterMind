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

public enum questionClass
{
    oneChoice,
    multiChoice,
    textInput
}

[Serializable]
public class Question
{
    public string question;
    public List<string> answers;
    public string comment;
    public string correctAnswer;
    public questionClass questionClass;
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
    public GameObject answerMCCorPrefab;
    public GameObject answerMCIncorPrefab;
    public GameObject acceptButtonPrefab;


    [Header("Кнопки вопросов")]

    public Sprite qButtPressed;
    public Sprite qButtDefault;
    public Sprite qButtComplete;


    [Header("Кнопки ответов")]

    public Sprite ansButtPressedCorrect;
    public Sprite ansButtPressedIncorrect;
    public Sprite ansButtDefault;
    public Sprite ansButtMCpressed;

    [Header("Настройка текста")]

    public float ansButtonDistance = 150;
    public float qButtonDistance = 80;
    public int ansButt2QuestDistance = 20;
    public int ansButtSizeModifier = 20;
    public float comment2LastAnsGap = 100;
    public float ansButtBaseHeight = 400;

    [Header("Разное")]
    public TextMeshProUGUI TextMeshProUGUI;
    public GameObject scrollBar;
    public List<GameObject> qButtons = new();
    public int currentQuestion = 0;
    public List<bool> firstPresses = new List<bool>();
    public HashSet<char> pressedAnswers = new HashSet<char>();
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

    public void MCcorrectOption()
    {
        var thisButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        var thisAnsLetter = thisButton.GetComponentInChildren<TextMeshProUGUI>().text[0];
        if (firstPresses[currentQuestion])
        {
            if (!pressedAnswers.Contains(thisAnsLetter))
            {
                pressedAnswers.Add(thisAnsLetter);
                thisButton.GetComponent<Image>().sprite = ansButtMCpressed;
            }
            else
            {
                pressedAnswers.Remove(thisAnsLetter);
                thisButton.GetComponent<Image>().sprite = ansButtDefault;
            }
        }
    }

    public void MCIncorrectOption()
    {
        var thisButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        var thisAnsLetter = thisButton.GetComponentInChildren<TextMeshProUGUI>().text[0];
        if (firstPresses[currentQuestion])
        {
            if (!pressedAnswers.Contains(thisAnsLetter))
            {
                pressedAnswers.Add(thisAnsLetter);
                thisButton.GetComponent<Image>().sprite = ansButtMCpressed;
            }
            else
            {
                pressedAnswers.Remove(thisAnsLetter);
                thisButton.GetComponent<Image>().sprite = ansButtDefault;
            }
        }
    }

    public void MCAccept()
    {
        if (firstPresses[currentQuestion])
        {
            firstPresses[currentQuestion] = false;
            var correctAnswerButtons = findAllCorrectAnswersMC();
            var correctAnswers = questions[currentQuestion].correctAnswer.Split(' ');
            foreach (var correctAnswerButton in correctAnswerButtons)
            {
                correctAnswerButton.image.sprite = ansButtPressedCorrect;
            }
            if (isCorrectAnswerMC(correctAnswers))
            {
                score1.score++;
            }
            else
            {
                score1.errorCount++;
            }
            Score tempScore = findCorrectAnswerAmount(correctAnswers);
            TextMeshProUGUI.color = Color.white;
            TextMeshProUGUI.text = $"Правильных ответов:{tempScore.score}/{correctAnswers.Length}\nНеправильных ответов: {tempScore.errorCount}\n" + TextMeshProUGUI.text;
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

        pressedAnswers = new();
    }
    public List<Button> findAllCorrectAnswers()
    {
        var buttons = this.gameObject.transform.GetComponentsInChildren<Button>(true);
        List<Button> correctButtons = new();

        for (int i = 0; i < buttons.Length;i++)
        {
            if (buttons[i].onClick.GetPersistentMethodName(0) == "correct_oc")
            {
                //print(button.name + button.onClick.GetPersistentMethodName(0));
                correctButtons.Add(buttons[i]);
            }
            //print(button.name + button.onClick.GetPersistentMethodName(0));
        }
        return correctButtons;
    }
    public List<Button> findAllCorrectAnswersMC()
    {
        var buttons = this.gameObject.transform.GetComponentsInChildren<Button>();
        List<Button> correctButtons = new();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].onClick.GetPersistentMethodName(0) == "correct")
            {
                //print(button.name + button.onClick.GetPersistentMethodName(0));
                correctButtons.Add(buttons[i]);
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

            switch (question.questionClass) { 

                case questionClass.oneChoice: 
                for (int i = 0; i < question.answers.Count; i++)
                {            
                    var answer = question.answers[i];
                    float buttHeightModif = TextMeshProUGUI.preferredHeight + ansButt2QuestDistance; //(question.question.Length/18)*ansButt2QuestDistance;
                    GameObject thisButton;
                    if (answer[0] == question.correctAnswer[0])
                    {
                        thisButton = Instantiate(answerButtPrefabCorr, questionObjects.Last().transform);
                    }
                    else
                    {
                        thisButton = Instantiate(answerButtPrefabIncorr, questionObjects.Last().transform);
                    }

                    if (i == 0)
                    {
                        lastButton = thisButton;
                        thisButton.transform.position -= new Vector3(0,buttHeightModif,0);
                    }
                    else
                    {

                    }
                    thisButton.GetComponent<testAnsButtScript>().testManager = this;
                    thisButton.GetComponentInChildren<TextMeshProUGUI>().text = answer;

                    var buttSizeModifHeight = thisButton.GetComponentInChildren<TextMeshProUGUI>().preferredHeight;
                    var buttSizeModifWidth = thisButton.GetComponent<RectTransform>().sizeDelta.x;
                    thisButton.GetComponent<RectTransform>().sizeDelta = new Vector2(buttSizeModifWidth,buttSizeModifHeight + ansButtSizeModifier);
                    var lastButtSize = lastButton.GetComponent<RectTransform>().sizeDelta;
                    var thisButtSize = thisButton.GetComponent<RectTransform>().sizeDelta;

                    thisButton.transform.position = lastButton.transform.position - new Vector3(0,lastButtSize.y/2 + thisButtSize.y/2 + ansButtonDistance,0);
                    //thisButton.transform.position -= new Vector3(0,buttSize.y/2 + buttHeightModif + ansButtonDistance * i, 0);
                    lastButton = thisButton;
                }
                break;
                case questionClass.multiChoice:
                    var butt = Instantiate(answerButtPrefabCorr, questionObjects.Last().transform);//кнопка-костыль
                    butt.SetActive(false);

                    butt = Instantiate(acceptButtonPrefab, questionObjects.Last().transform);
                    butt.GetComponent<testAnsButtMCScript>().testManager = this;

                    var correctLetters = question.correctAnswer.Split(' ');
                    for (int i = 0; i < question.answers.Count; i++)
                    {
                        var answer = question.answers[i];
                        float buttHeightModif = TextMeshProUGUI.preferredHeight + ansButt2QuestDistance;
                        GameObject thisButton;
                        if (isCorrectChoice(correctLetters, answer[0]))
                        {
                            thisButton = Instantiate(answerMCCorPrefab, questionObjects.Last().transform);
                        }
                        else
                        {
                            thisButton = Instantiate(answerMCIncorPrefab, questionObjects.Last().transform);
                        }
                        thisButton.GetComponent<testAnsButtMCScript>().testManager = this;
                        thisButton.GetComponentInChildren<TextMeshProUGUI>().text = answer;

                        if (i == 0)
                        {
                            lastButton = thisButton;
                            thisButton.transform.position -= new Vector3(0, buttHeightModif, 0);
                        }
                        else
                        {

                        }
                        var buttSizeModifHeight = thisButton.GetComponentInChildren<TextMeshProUGUI>().preferredHeight;
                        var buttSizeModifWidth = thisButton.GetComponent<RectTransform>().sizeDelta.x;
                        thisButton.GetComponent<RectTransform>().sizeDelta = new Vector2(buttSizeModifWidth, buttSizeModifHeight + ansButtSizeModifier);
                        var lastButtSize = lastButton.GetComponent<RectTransform>().sizeDelta;
                        var thisButtSize = thisButton.GetComponent<RectTransform>().sizeDelta;

                        thisButton.transform.position = lastButton.transform.position - new Vector3(0, lastButtSize.y / 2 + thisButtSize.y / 2 + ansButtonDistance, 0);

                        lastButton = thisButton;
                    }
                    break;

                case questionClass.textInput: 
                    
                break;

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
    public bool isCorrectChoice(string[] correctAnswers, char givenAnswer)
    {
        foreach (var correctAnswer in correctAnswers)
        {
            if (givenAnswer.Equals(correctAnswer[0])) return true;
        }
        return false;
    }
    public bool isCorrectAnswerMC(string[] correctAnswers)
    {
        char[] charAns = new char[correctAnswers.Length];
        for(int i=0;i<correctAnswers.Length;i++)
        {
            charAns[i] = correctAnswers[i][0];
        }
        return pressedAnswers.SetEquals(charAns);
    }
    public Score findCorrectAnswerAmount(string[] correctAnswers)
    {
        Score amount = new();

        char[] charAnsCorr = new char[correctAnswers.Length];
        HashSet<char> corrAnsSet = new HashSet<char>();
        for (int i = 0; i < correctAnswers.Length; i++)
        {
            charAnsCorr[i] = correctAnswers[i][0];
            corrAnsSet.Add(charAnsCorr[i]);
            print(charAnsCorr[i]);
        }
        amount.score = pressedAnswers.Intersect(corrAnsSet).Count();
        amount.errorCount = pressedAnswers.Count() - amount.score;
        print($"Correct answers: {amount.score}\n Incorrect answers: {amount.errorCount}");
        return amount;
    }
}
