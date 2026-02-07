using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public enum QuestionType { OneChoice, MultiChoice, TextInput }

[System.Serializable]
public class Question
{
    public string question;
    public QuestionType type;
    public List<string> answers; // Для выбора
    public List<int> correctAnswerIndex; // Индексы правильных ответов
    public string correctAnswer; // Для текстового ввода
    public string comment;
    public bool isAnswered = false;
    public List<int> playerSelectedIndices = new List<int>();
}

[System.Serializable]
public class Score
{
    public int score = 0;
    public int errorCount = 0;
}

public class TestManager2 : MonoBehaviour
{
    public Score sessionScore = new Score();
    bool isCorrect = false;

    [Header("Data")]
    public List<Question> questions;
    private int currentQuestionIndex = 0;
    private List<Image> navButtonsImages = new List<Image>();
    private List<int> selectedIndices = new List<int>();

    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public Transform optionsParent;
    public GameObject winCanvas;  
    public GameObject loseCanvas;
    public TMP_InputField inputField;
    public Button submitButton;
    public Button nextButton;
    public TextMeshProUGUI commentText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreText2;

    [Header("Navigation Panel")]
    public GameObject navButtonPrefab; // Префаб маленькой кнопки номера
    public Transform navPanelParent;   // Сюда они спавнятся (Horizontal Layout Group)

    [Header("Prefabs")]
    public GameObject optionButtonPrefab; // Всего ОДИН префаб кнопки

    [Header("Settings")]
    public Color defaultColor = Color.white;
    public Color selectedColor = new Color(0.7f, 0.7f, 1f); // Голубоватый при нажатии
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color missedCorrectColor = new Color(0.2f, 0.5f, 0.2f);
    public Color activeNavColor = Color.blue;
    public Color inactiveNavColor = Color.blue;
    public Color answeredNavColor = Color.blue;

    void Start()
    {
        CreateNavigationPanel();
        ShowQuestion(0);

        // Вешаем логику на кнопки управления
        submitButton.onClick.AddListener(CheckAnswer);
        nextButton.onClick.AddListener(OnNextClick);
    }

    void CreateNavigationPanel()
    {
        // Очищаем панель перед созданием
        foreach (Transform child in navPanelParent) Destroy(child.gameObject);
        navButtonsImages.Clear();

        for (int i = 0; i < questions.Count; i++)
        {
            int index = i;
            GameObject go = Instantiate(navButtonPrefab, navPanelParent);
            go.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();

            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => ShowQuestion(index));

            navButtonsImages.Add(go.GetComponent<Image>());
        }
    }

    public void ShowQuestion(int index)
    {
        currentQuestionIndex = index;
        Question q = questions[index];
        selectedIndices.Clear();

        questionText.text = q.question;
        inputField.gameObject.SetActive(q.type == QuestionType.TextInput);

        // Очистка старых кнопок
        foreach (Transform child in optionsParent) Destroy(child.gameObject);

        if (q.type != QuestionType.TextInput)
        {
            for (int i = 0; i < q.answers.Count; i++)
            {
                int optionIndex = i;
                GameObject btnObj = Instantiate(optionButtonPrefab, optionsParent);
                btnObj.GetComponentInChildren<TMP_Text>().text = q.answers[i];
                btnObj.GetComponent<Image>().color = defaultColor;

                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(() => OnOptionClick(optionIndex));
            }
        }

        // ЛОГИКА ДЛЯ УЖЕ ОТВЕЧЕННЫХ ВОПРОСОВ
        if (q.isAnswered)
        {
            commentText.text = q.comment;
            commentText.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(false);

            if (q.type == QuestionType.TextInput)
            {
                inputField.text = q.correctAnswer; // Можно показать правильный ответ в поле
                inputField.interactable = false;
            }
            else
            {
                // Используем сохраненные индексы игрока для подсветки
                HighlightButtons(q.playerSelectedIndices);
            }
        }
        else
        {
            // Для новых вопросов сбрасываем состояние
            commentText.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(q.type != QuestionType.OneChoice);
            inputField.interactable = true;
            inputField.text = "";
        }

        UpdateNavUI(index);
        LayoutRebuilder.ForceRebuildLayoutImmediate(optionsParent.GetComponent<RectTransform>());
    }

    void UpdateNavUI(int index)
    {
        for (int i = 0; i < navButtonsImages.Count; i++)
        {
            if (i == index) navButtonsImages[i].color = activeNavColor;
            else if (questions[i].isAnswered) navButtonsImages[i].color = answeredNavColor;
            else navButtonsImages[i].color = inactiveNavColor;
        }
    }

    void OnOptionClick(int index)
    {
        Question q = questions[currentQuestionIndex];
        if (q.isAnswered) return;

        if (q.type == QuestionType.OneChoice)
        {
            q.playerSelectedIndices = new List<int> { index }; // Сохраняем выбор
            bool isCorrect = q.correctAnswerIndex.Contains(index);
            HighlightButtons(q.playerSelectedIndices);
            Validate(isCorrect);
        }
        else if (q.type == QuestionType.MultiChoice)
        {
            // Переключаем выбор
            if (selectedIndices.Contains(index))
                selectedIndices.Remove(index);
            else
                selectedIndices.Add(index);

            // Визуально подсвечиваем выбранные (пока не нажата "Принять")
            for (int i = 0; i < optionsParent.childCount; i++)
            {
                optionsParent.GetChild(i).GetComponentInChildren<Image>().color =
                    selectedIndices.Contains(i) ? selectedColor : defaultColor;
            }
        }
    }

    public void CheckAnswer()
    {
        Question q = questions[currentQuestionIndex];
        if (q.isAnswered) return;

        bool isCorrect = false;

        if (q.type == QuestionType.TextInput)
        {
            isCorrect = (inputField.text.ToLower().Trim() == q.correctAnswer.ToLower().Trim());
            Validate(isCorrect);
        }
        else if (q.type == QuestionType.MultiChoice)
        {
            q.playerSelectedIndices = new List<int>(selectedIndices); // Сохраняем выбор
            var correctList = q.correctAnswerIndex;
            isCorrect = selectedIndices.Count == correctList.Count && !selectedIndices.Except(correctList).Any();

            HighlightButtons(q.playerSelectedIndices);
            Validate(isCorrect);
        }
    }

    void HighlightButtons(List<int> playerChoices)
    {
        Question q = questions[currentQuestionIndex];
        for (int i = 0; i < optionsParent.childCount; i++)
        {
            Image img = optionsParent.GetChild(i).GetComponentInChildren<Image>();
            bool isCorrectIdx = q.correctAnswerIndex.Contains(i);
            bool isSelected = playerChoices.Contains(i);

            if (isSelected && isCorrectIdx) img.color = correctColor; // Правильно выбрал
            else if (isSelected && !isCorrectIdx) img.color = wrongColor; // Ошибся
            else if (!isSelected && isCorrectIdx) img.color = missedCorrectColor; // Не выбрал правильный (подсказка)
            else img.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Остальные затемняем

            // Выключаем кнопку после ответа
            optionsParent.GetChild(i).GetComponentInChildren<Button>().interactable = false;
        }
    }

    void Validate(bool isCorrect)
    {
        questions[currentQuestionIndex].isAnswered = true;

        if (isCorrect) sessionScore.score++;
        else sessionScore.errorCount++;

        commentText.text = questions[currentQuestionIndex].comment;
        commentText.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(false);

        UpdateNavUI(currentQuestionIndex);
    }

    public void RestartTest()
    {
        sessionScore.score = 0;
        sessionScore.errorCount = 0;

        foreach (var q in questions)
        {
            q.isAnswered = false;
            q.playerSelectedIndices.Clear(); // Очищаем историю нажатий
        }

        if (winCanvas != null) winCanvas.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(false);

        ShowQuestion(0);
    }

    void OnNextClick()
    {
        int firstUnanswered = questions.FindIndex(q => !q.isAnswered);

        if (firstUnanswered == -1) // Если ответили на все вопросы
        {
            // 1. Сначала считаем, какой канвас показать
            // В данном примере 7 баллов и выше — это победа
            bool isWin = sessionScore.score >= 7;

            // 2. Активируем нужный и выключаем ненужный
            if (winCanvas != null) winCanvas.SetActive(isWin);
            if (loseCanvas != null) loseCanvas.SetActive(!isWin);

            // 3. Обновляем текст результата
            // (Убедитесь, что scoreText есть на обоих канвасах или он общий)
            if (scoreText != null)
                scoreText.text = $"Ваш результат: \n{sessionScore.score}/{questions.Count}";
            if (scoreText2 != null)
                scoreText2.text = $"Ваш результат: \n{sessionScore.score}/{questions.Count}";

            return;
        }

        if (currentQuestionIndex == questions.Count - 1)
        {
            ShowQuestion(firstUnanswered);
            return;
        }

        if (questions[currentQuestionIndex + 1].isAnswered) ShowQuestion(firstUnanswered);
        else ShowQuestion(currentQuestionIndex + 1);
    }
}