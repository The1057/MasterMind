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
    public List<string> answers;
    public List<int> correctAnswerIndex;
    public string correctAnswer;
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
    private List<TMP_Text> navButtonsTexts = new List<TMP_Text>(); // ДОБАВЛЕНО: для текста кнопок
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
    public GameObject navButtonPrefab;
    public Transform navPanelParent;

    [Header("Prefabs")]
    public GameObject optionButtonPrefab;

    [Header("Settings")]
    public Color defaultColor = Color.white;
    public Color selectedColor = new Color(0.7f, 0.7f, 1f);
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color missedCorrectColor = new Color(0.2f, 0.5f, 0.2f);
    public Color activeNavColor = Color.blue;
    public Color inactiveNavColor = Color.blue;
    public Color answeredNavColor = Color.blue;

    // ДОБАВЛЕНО: три цвета для текста навигационных кнопок
    [Header("Navigation Button Text Colors")]
    public Color currentNavTextColor = Color.white;
    public Color unansweredNavTextColor = Color.gray;
    public Color answeredNavTextColor = Color.green;

    [Header("Outline Colors for Navigation Buttons")]
    public Color currentNavOutlineColor = Color.white;
    public Color unansweredNavOutlineColor = Color.gray;
    public Color answeredNavOutlineColor = Color.green;

    [Header("Outline Colors for Answer Buttons")]
    public Color defaultOutlineColor = Color.white;
    public Color selectedOutlineColor = new Color(0.7f, 0.7f, 1f);
    public Color correctOutlineColor = Color.green;
    public Color wrongOutlineColor = Color.red;
    public Color missedCorrectOutlineColor = new Color(0.2f, 0.5f, 0.2f);

    void Start()
    {
        CreateNavigationPanel();
        ShowQuestion(0);
        submitButton.onClick.AddListener(CheckAnswer);
        nextButton.onClick.AddListener(OnNextClick);
    }

    void CreateNavigationPanel()
    {
        foreach (Transform child in navPanelParent) Destroy(child.gameObject);
        navButtonsImages.Clear();
        navButtonsTexts.Clear(); // ДОБАВЛЕНО

        for (int i = 0; i < questions.Count; i++)
        {
            int index = i;
            GameObject go = Instantiate(navButtonPrefab, navPanelParent);
            TMP_Text txt = go.GetComponentInChildren<TMP_Text>();
            txt.text = (i + 1).ToString();

            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => ShowQuestion(index));

            navButtonsImages.Add(go.GetComponent<Image>());
            navButtonsTexts.Add(txt); // ДОБАВЛЕНО: сохраняем текст
        }
    }

    public void ShowQuestion(int index)
    {
        currentQuestionIndex = index;
        Question q = questions[index];
        selectedIndices.Clear();

        questionText.text = q.question;
        inputField.gameObject.SetActive(q.type == QuestionType.TextInput);

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

        if (q.isAnswered)
        {
            commentText.text = q.comment;
            commentText.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(false);

            if (q.type == QuestionType.TextInput)
            {
                inputField.text = q.correctAnswer;
                inputField.interactable = false;
            }
            else
            {
                HighlightButtons(q.playerSelectedIndices);
            }
        }
        else
        {
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
            // Пример для навигационных кнопок (в UpdateNavUI)
            Outline outline = navButtonsImages[i].GetComponent<Outline>();
            if (outline != null)
            {
                if (i == index) outline.effectColor = currentNavOutlineColor;
                else if (questions[i].isAnswered) outline.effectColor = answeredNavOutlineColor;
                else outline.effectColor = unansweredNavOutlineColor;
            }
            // Цвет фона (как было)
            if (i == index) navButtonsImages[i].color = activeNavColor;
            else if (questions[i].isAnswered) navButtonsImages[i].color = answeredNavColor;
            else navButtonsImages[i].color = inactiveNavColor;

            // ДОБАВЛЕНО: цвет текста в зависимости от состояния
            if (i == index)
                navButtonsTexts[i].color = currentNavTextColor;
            else if (questions[i].isAnswered)
                navButtonsTexts[i].color = answeredNavTextColor;
            else
                navButtonsTexts[i].color = unansweredNavTextColor;
        }
    }

    void OnOptionClick(int index)
    {
        Question q = questions[currentQuestionIndex];
        if (q.isAnswered) return;

        if (q.type == QuestionType.OneChoice)
        {
            q.playerSelectedIndices = new List<int> { index };
            bool isCorrect = q.correctAnswerIndex.Contains(index);

            // Подсвечиваем выбранный вариант сразу (без ожидания кнопки "Принять")
            for (int i = 0; i < optionsParent.childCount; i++)
            {
                Transform btnTransform = optionsParent.GetChild(i);
                Image img = btnTransform.GetComponent<Image>();
                Outline outline = btnTransform.GetComponent<Outline>();
                Button btn = btnTransform.GetComponent<Button>();

                if (i == index)
                {
                    // Выбранный вариант
                    if (isCorrect)
                    {
                        img.color = correctColor;
                        if (outline != null) outline.effectColor = correctOutlineColor;
                    }
                    else
                    {
                        img.color = wrongColor;
                        if (outline != null) outline.effectColor = wrongOutlineColor;
                    }
                }
                else
                {
                    // Остальные варианты затемняем
                    img.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    if (outline != null) outline.effectColor = defaultOutlineColor;
                }

                btn.interactable = false;
            }

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
                Transform btnTransform = optionsParent.GetChild(i);
                Image img = btnTransform.GetComponent<Image>();
                Outline outline = btnTransform.GetComponent<Outline>();

                bool isSelected = selectedIndices.Contains(i);

                if (isSelected)
                {
                    img.color = selectedColor;
                    if (outline != null) outline.effectColor = selectedOutlineColor;
                }
                else
                {
                    img.color = defaultColor;
                    if (outline != null) outline.effectColor = defaultOutlineColor;
                }
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
            q.playerSelectedIndices = new List<int>(selectedIndices);
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
            Transform btnTransform = optionsParent.GetChild(i);
            Image img = btnTransform.GetComponent<Image>();
            Outline outline = btnTransform.GetComponent<Outline>();

            bool isCorrectIdx = q.correctAnswerIndex.Contains(i);
            bool isSelected = playerChoices.Contains(i);

            Color fillColor;
            Color outlineColor;

            if (isSelected && isCorrectIdx)
            {
                fillColor = correctColor;
                outlineColor = correctOutlineColor;
            }
            else if (isSelected && !isCorrectIdx)
            {
                fillColor = wrongColor;
                outlineColor = wrongOutlineColor;
            }
            else if (!isSelected && isCorrectIdx)
            {
                fillColor = missedCorrectColor;
                outlineColor = missedCorrectOutlineColor;
            }
            else
            {
                fillColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                outlineColor = defaultOutlineColor;
            }

            img.color = fillColor;
            if (outline != null) outline.effectColor = outlineColor;

            btnTransform.GetComponent<Button>().interactable = false;
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
            q.playerSelectedIndices.Clear();
        }

        if (winCanvas != null) winCanvas.SetActive(false);
        if (loseCanvas != null) loseCanvas.SetActive(false);

        ShowQuestion(0);
    }

    void OnNextClick()
    {
        int firstUnanswered = questions.FindIndex(q => !q.isAnswered);

        if (firstUnanswered == -1)
        {
            bool isWin = sessionScore.score >= 7;
            if (winCanvas != null) winCanvas.SetActive(isWin);
            if (loseCanvas != null) loseCanvas.SetActive(!isWin);
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