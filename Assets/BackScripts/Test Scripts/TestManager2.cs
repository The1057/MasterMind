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
using UnityEngine.SceneManagement;

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
    private List<GameObject> questionObjects = new();


    [Header("Префабы")]
    public GameObject QuestionPrefab;
    public GameObject answerButtPrefabIncorr;
    public GameObject answerButtPrefabCorr;
    public GameObject scrollPrefab;
    public GameObject qButtonPrefab;
    public GameObject answerMCCorPrefab;
    public GameObject answerMCIncorPrefab;
    public GameObject acceptButtonPrefab;
    public GameObject textInputPrefab;
    public GameObject checkAnswerButtonPrefab;
    public GameObject nextQuestionButtonPrefab;


    [Header("Кнопки вопросов")]

    public Sprite qButtPressed;
    public Sprite qButtDefault;
    public Sprite qButtComplete;


    [Header("Кнопки ответов")]
    public Sprite ansButtPressedCorrect;
    public Sprite ansButtPressedIncorrect;
    public Sprite ansButtDefault;
    public Sprite ansButtMCpressed;
    public Sprite IFAcceptGrey;
    public Sprite IFAcceptPink;

    [Header("Настройка текста")]
    public float ansButtonDistance = 150;
    public float qButtonDistance = 80;
    public int ansButt2QuestDistance = 20;
    public float inputField2QuestDist = 100;
    public int ansButtSizeModifier = 20;
    public float comment2LastAnsGap = 100;
    public float ansButtBaseHeight = 400;

    [Header("Разное")]
    private TextMeshProUGUI TextMeshProUGUI;
    private GameObject scrollBar;
    private List<GameObject> qButtons = new();
    public int currentQuestion = 0;
    public List<bool> firstPresses = new List<bool>();
    public HashSet<char> pressedAnswers = new HashSet<char>();
    private List<Button> correctButtons;
    public Score score1 = new Score();
    public float nextButtonOffsetX = -20f;
    public float nextButtonOffsetY = 20f;

    [Header("Позиция кнопки 'Следующий вопрос'")]
    public Vector2 nextButtonAnchorMin = new Vector2(0.5f, 0); // Анкор по умолчанию: центр снизу
    public Vector2 nextButtonAnchorMax = new Vector2(0.5f, 0);
    public Vector2 nextButtonOffset = new Vector2(0, 20f); // Отступ от анкора
    public Vector2 nextButtonSize = new Vector2(100f, 50f); // Размер кнопки

    [Header("Скролл-бар")]
    public Transform scrollContent;

    [Header("Завершение теста")]
    public GameObject finishCanvas;
    public string archiveSceneName = "Archive";
    public TextMeshProUGUI scoreText;

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
            // Обновляем TextMeshProUGUI перед использованием
            TextMeshProUGUI = questionObjects[currentQuestion].transform.Find("Пояснение").GetComponent<TextMeshProUGUI>();
            if (TextMeshProUGUI != null)
            {
                TextMeshProUGUI.color = Color.white;
            }

            // Показываем стрелку
            ShowNextButton();
        }
    }

    private void ShowNextButton()
    {
        // Ищем кнопку по имени с учетом "(Clone)"
        Transform nextButton = questionObjects[currentQuestion].transform.Find("NextQuestionButton");
        if (nextButton == null)
        {
            // Если не нашли, пробуем найти по компоненту
            Button[] buttons = questionObjects[currentQuestion].GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                if (button.name == "NextQuestionButton" || button.name.StartsWith("NextQuestionButton"))
                {
                    nextButton = button.transform;
                    break;
                }
            }
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    public void incorrectOption()
    {
        if (firstPresses[currentQuestion])
        {
            var thisButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            thisButton.GetComponent<Image>().sprite = ansButtPressedIncorrect;
            if (correctButtons != null && correctButtons.Count > currentQuestion)
            {
                correctButtons[currentQuestion].image.sprite = ansButtPressedCorrect;
            }
        }

        if (firstPresses[currentQuestion])
        {
            score1.errorCount++;
            firstPresses[currentQuestion] = false;
            // Обновляем TextMeshProUGUI перед использованием
            TextMeshProUGUI = questionObjects[currentQuestion].transform.Find("Пояснение").GetComponent<TextMeshProUGUI>();
            if (TextMeshProUGUI != null)
            {
                TextMeshProUGUI.color = Color.white;
            }

            // Показываем стрелку
            ShowNextButton();
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
            // Обновляем TextMeshProUGUI перед использованием
            TextMeshProUGUI = questionObjects[currentQuestion].transform.Find("Пояснение").GetComponent<TextMeshProUGUI>();
            if (TextMeshProUGUI != null)
            {
                TextMeshProUGUI.color = Color.white;
                TextMeshProUGUI.text = $"Правильных ответов:{tempScore.score}/{correctAnswers.Length}\nНеправильных ответов: {tempScore.errorCount}\n" + TextMeshProUGUI.text;
            }

            // Показываем стрелку
            ShowNextButton();
        }
    }

    public void IFCheckAnswer()
    {
        if (!firstPresses[currentQuestion]) return;

        TMP_InputField inputField = questionObjects[currentQuestion].GetComponentInChildren<TMP_InputField>();
        if (inputField == null) return;

        string inputText = inputField.text;

        firstPresses[currentQuestion] = false;

        string correctAnswer = questions[currentQuestion].correctAnswer.Trim().ToLower();
        string userAnswer = inputText.Trim().ToLower();

        bool isCorrect = userAnswer == correctAnswer;

        // Подсветка поля ввода
        if (isCorrect)
        {
            score1.score++;
        }
        else
        {
            score1.errorCount++;
        }
        inputField.interactable = false;

        // Обновляем TextMeshProUGUI перед использованием
        TextMeshProUGUI = questionObjects[currentQuestion].transform.Find("Пояснение").GetComponent<TextMeshProUGUI>();
        if (TextMeshProUGUI != null)
        {
            TextMeshProUGUI.color = Color.white;
            TextMeshProUGUI.text = (isCorrect ? "Правильно!" : "Неправильно!") + "\n" + questions[currentQuestion].comment;
        }

        // Показываем стрелку
        ShowNextButton();
    }
    public void setQuestion(GameObject button)
    {
        int newIndex = int.Parse(button.GetComponentInChildren<TextMeshProUGUI>().text) - 1;
        int oldQuestion = currentQuestion;
        currentQuestion = newIndex;
        questionObjects[currentQuestion].SetActive(true);
        if (oldQuestion != currentQuestion && oldQuestion >= 0)
        {
            questionObjects[oldQuestion].SetActive(false);
        }

        // Обновляем TextMeshProUGUI после активации
        TextMeshProUGUI = questionObjects[currentQuestion].transform.Find("Пояснение").GetComponent<TextMeshProUGUI>();

        var images = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] == null) continue; // Проверка на null для безопасности
            if (i == currentQuestion)
            {
                images[i].sprite = qButtPressed;
                this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
            else if (!firstPresses[i])
            {
                images[i].sprite = qButtComplete;
                this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
            else
            {
                images[i].sprite = qButtDefault;
                this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            }
        }

        pressedAnswers = new();
    }
    public List<Button> findAllCorrectAnswers()
    {
        var buttons = this.gameObject.transform.GetComponentsInChildren<Button>(true);
        List<Button> correctButtons = new();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].onClick.GetPersistentEventCount() > 0 && buttons[i].onClick.GetPersistentMethodName(0) == "correct_oc")
            {
                correctButtons.Add(buttons[i]);
            }
        }
        return correctButtons;
    }
    public List<Button> findAllCorrectAnswersMC()
    {
        var buttons = this.gameObject.transform.GetComponentsInChildren<Button>();
        List<Button> correctButtons = new();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].onClick.GetPersistentEventCount() > 0 && buttons[i].onClick.GetPersistentMethodName(0) == "correct")
            {
                correctButtons.Add(buttons[i]);
            }
        }
        return correctButtons;
    }
    public void generateTest()
    {
        for (int questionIndex = 0; questionIndex < questions.Count; questionIndex++)
        {
            Question question = questions[questionIndex];
            GameObject currentQuestionObj = Instantiate(QuestionPrefab, this.transform);
            questionObjects.Add(currentQuestionObj);

            TextMeshProUGUI questionText = currentQuestionObj.transform.Find("Вопрос").GetComponent<TextMeshProUGUI>();
            questionText.text = question.question;

            GameObject lastButton = null;

            switch (question.questionClass)
            {

                case questionClass.oneChoice:
                    for (int i = 0; i < question.answers.Count; i++)
                    {
                        var answer = question.answers[i];
                        float buttHeightModif = questionText.preferredHeight + ansButt2QuestDistance;
                        GameObject thisButton;
                        if (answer[0] == question.correctAnswer[0])
                        {
                            thisButton = Instantiate(answerButtPrefabCorr, currentQuestionObj.transform);
                        }
                        else
                        {
                            thisButton = Instantiate(answerButtPrefabIncorr, currentQuestionObj.transform);
                        }

                        if (i == 0)
                        {
                            lastButton = thisButton;
                            thisButton.transform.position -= new Vector3(0, buttHeightModif, 0);
                        }
                        thisButton.GetComponent<testAnsButtScript>().testManager = this;
                        thisButton.GetComponentInChildren<TextMeshProUGUI>().text = answer;

                        var buttSizeModifHeight = thisButton.GetComponentInChildren<TextMeshProUGUI>().preferredHeight;
                        var buttSizeModifWidth = thisButton.GetComponent<RectTransform>().sizeDelta.x;
                        thisButton.GetComponent<RectTransform>().sizeDelta = new Vector2(buttSizeModifWidth, buttSizeModifHeight + ansButtSizeModifier);
                        var lastButtSize = lastButton.GetComponent<RectTransform>().sizeDelta;
                        var thisButtSize = thisButton.GetComponent<RectTransform>().sizeDelta;

                        thisButton.transform.position = lastButton.transform.position - new Vector3(0, lastButtSize.y / 2 + thisButtSize.y / 2 + ansButtonDistance, 0);
                        lastButton = thisButton;
                    }
                    break;
                case questionClass.multiChoice:
                    var butt = Instantiate(answerButtPrefabCorr, currentQuestionObj.transform);//кнопка-костыль
                    butt.SetActive(false);

                    butt = Instantiate(acceptButtonPrefab, currentQuestionObj.transform);
                    butt.GetComponent<testAnsButtMCScript>().testManager = this;

                    var correctLetters = question.correctAnswer.Split(' ');
                    for (int i = 0; i < question.answers.Count; i++)
                    {
                        var answer = question.answers[i];
                        float buttHeightModif = questionText.preferredHeight + ansButt2QuestDistance;
                        GameObject thisButton;
                        if (isCorrectChoice(correctLetters, answer[0]))
                        {
                            thisButton = Instantiate(answerMCCorPrefab, currentQuestionObj.transform);
                        }
                        else
                        {
                            thisButton = Instantiate(answerMCIncorPrefab, currentQuestionObj.transform);
                        }
                        thisButton.GetComponent<testAnsButtMCScript>().testManager = this;
                        thisButton.GetComponentInChildren<TextMeshProUGUI>().text = answer;

                        if (i == 0)
                        {
                            lastButton = thisButton;
                            thisButton.transform.position -= new Vector3(0, buttHeightModif, 0);
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
                    {
                        // Кнопка-костыль
                        GameObject dummyButton = Instantiate(answerButtPrefabCorr, currentQuestionObj.transform);
                        dummyButton.SetActive(false);

                        // Получаем позицию вопроса
                        Vector3 questionPosition = questionText.transform.position;

                        // Создаем InputField
                        GameObject inputFieldGO = Instantiate(textInputPrefab, currentQuestionObj.transform);
                        TMP_InputField inputField = inputFieldGO.GetComponent<TMP_InputField>();

                        // Позиционируем InputField ниже вопроса
                        float offsetFromQuestion = questionText.preferredHeight + inputField2QuestDist;
                        inputFieldGO.transform.position = new Vector3(questionPosition.x, questionPosition.y - offsetFromQuestion, questionPosition.z);

                        lastButton = inputFieldGO;

                        // Создаем кнопку "Проверить"
                        GameObject checkButton = Instantiate(checkAnswerButtonPrefab, currentQuestionObj.transform);
                        checkButton.GetComponent<testAnsButtIFScript>().testManager = this;

                        // Позиционируем кнопку под InputField
                        var inputSize = inputFieldGO.GetComponent<RectTransform>().sizeDelta;
                        var buttonSize = checkButton.GetComponent<RectTransform>().sizeDelta;
                        checkButton.transform.position = new Vector3(
                            inputFieldGO.transform.position.x,
                            inputFieldGO.transform.position.y - (inputSize.y / 2 + buttonSize.y / 2 + ansButtonDistance),
                            inputFieldGO.transform.position.z
                        );

                        lastButton = checkButton;
                        break;
                    }

            }
            TextMeshProUGUI commentText = currentQuestionObj.transform.Find("Пояснение").GetComponent<TextMeshProUGUI>();
            commentText.text = question.comment;
            commentText.color = Color.clear;
            commentText.gameObject.transform.position = lastButton.transform.position
                - new Vector3(0, comment2LastAnsGap + lastButton.GetComponent<RectTransform>().sizeDelta.y, 0);

            // Создаём кнопку "Next" для всех вопросов
            GameObject nextButton = Instantiate(nextQuestionButtonPrefab, currentQuestionObj.transform);
            nextButton.name = "NextQuestionButton";
            nextButton.SetActive(false); // Скрыть изначально

            // Настройка RectTransform (твой текущий код, оставь как есть)
            RectTransform rect = nextButton.GetComponent<RectTransform>();
            rect.anchorMin = nextButtonAnchorMin;
            rect.anchorMax = nextButtonAnchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = nextButtonOffset;
            rect.sizeDelta = nextButtonSize;

            // Назначаем обработчик
            Button button = nextButton.GetComponent<Button>();
            int thisQuestionIndex = questionIndex; // Локальная переменная для замыкания
            button.onClick.AddListener(() =>
            {
                if (thisQuestionIndex == questions.Count - 1)
                {
                    if (finishCanvas != null)
                    {
                        finishCanvas.SetActive(true);
                        currentQuestionObj.SetActive(false); // Опционально
                        if (scoreText != null)
                        {
                            scoreText.text = $"Ваш результат \n{score1.score} / {questions.Count}"; // Обновляем текст
                        }
                    }
                }
                else
                {
                    // Обычное переключение
                    setQuestionByIndex(thisQuestionIndex + 1);
                }
            });


            firstPresses.Add(true);
            currentQuestionObj.SetActive(false);
        }

        scrollBar = GameObject.Find("TestScrollView");
        Transform content = scrollBar.transform.GetChild(0).GetChild(0);
        GameObject template = content.GetChild(0).gameObject;
        for (int i = 0; i < questions.Count; i++)
        {
            var thisButton = Instantiate(template, content);
            thisButton.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
            // Добавляем обработчик клика на кнопку скролл-бара
            Button btn = thisButton.GetComponent<Button>();
            GameObject buttonObj = thisButton; // Для замыкания
            btn.onClick.AddListener(() => setQuestion(buttonObj));
        }       //создаём и расставляем элементы скролл бара

        content.GetComponent<HorizontalLayoutGroup>().spacing = qButtonDistance;
        template.SetActive(false);
        Destroy(template); // Уничтожаем шаблон, чтобы избежать лишнего элемента

        currentQuestion = -1;
        setQuestion(content.GetChild(0).gameObject);

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
        for (int i = 0; i < correctAnswers.Length; i++)
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
    public void NextQuestion()
    {
        if (currentQuestion < questions.Count - 1)
        {
            setQuestionByIndex(currentQuestion + 1);
        }
    }

    private void setQuestionByIndex(int index)
    {
        if (index == currentQuestion) return; // Избегаем повторной активации/деактивации

        int oldQuestion = currentQuestion;
        currentQuestion = index;
        questionObjects[currentQuestion].SetActive(true);
        if (oldQuestion != currentQuestion && oldQuestion >= 0)
        {
            questionObjects[oldQuestion].SetActive(false);
        }

        // Обновляем отображение кнопок вопросов и цвет текста
        var scrollContent = this.gameObject.transform.GetChild(0).GetChild(0).GetChild(0); // Путь к содержимому скролл-бара
        if (scrollContent != null)
        {
            var images = scrollContent.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                var textComponent = images[i].transform.GetComponentInChildren<TextMeshProUGUI>();
                if (i == currentQuestion && textComponent != null)
                {
                    images[i].sprite = qButtPressed;
                    textComponent.color = Color.white;
                }
                else if (!firstPresses[i] && textComponent != null)
                {
                    images[i].sprite = qButtComplete;
                    textComponent.color = Color.black;
                }
                else if (textComponent != null)
                {
                    images[i].sprite = qButtDefault;
                    textComponent.color = Color.black;
                }
            }
        }

        // Устанавливаем TextMeshProUGUI локально для текущего вопроса
        TextMeshProUGUI = questionObjects[currentQuestion].transform.Find("Пояснение").GetComponent<TextMeshProUGUI>();
        pressedAnswers = new();
    }
}