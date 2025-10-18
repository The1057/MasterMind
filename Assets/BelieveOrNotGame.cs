using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BelieveOrNotGame : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject questionPanel;
    public GameObject resultPanel;
    public GameObject endPanel;

    public TMP_Text statementText;
    public TMP_Text timerText;
    public TMP_Text feedbackText;
    public TMP_Text explanationText;

    public Button believeButton;
    public Button notBelieveButton;
    public Button nextButton;
    public Button restartButton;

    [Header("Statements")]
    public List<StatementData> statements;

    private int currentStatementIndex = 0;
    private float timeLeft;
    private bool isAnswered = false;
    private bool isGameActive = false;

    private const float INITIAL_TIME = 6f;
    private List<StatementData> currentStatements;

    [Header("End Game UI")]
    public TMP_Text resultText; // <-- Новое поле для отображения результата

    private int correctAnswers = 0;
    void Start()
    {
        // Подписываем кнопки
        believeButton.onClick.AddListener(() => OnAnswerSelected(true));
        notBelieveButton.onClick.AddListener(() => OnAnswerSelected(false));
        nextButton.onClick.AddListener(ShowNextStatement);
        restartButton.onClick.AddListener(RestartGame);

        RestartGame();
    }

    void ShowNextStatement()
    {
        if (currentStatementIndex >= statements.Count)
        {
            // Конец игры
            EndGame();
            return;
        }

        isAnswered = false;
        questionPanel.SetActive(true);
        resultPanel.SetActive(false);

        StatementData current = currentStatements[currentStatementIndex];
        statementText.text = current.statement;

        // Устанавливаем таймер
        timeLeft = INITIAL_TIME;
        StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        while (timeLeft > 0 && !isAnswered)
        {
            timerText.text = Mathf.Ceil(timeLeft).ToString() + "с";
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        if (!isAnswered)
        {
            isAnswered = true;
            ProcessAnswer(false);
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    void OnAnswerSelected(bool playerBelieves)
    {
        if (isAnswered) return;
        isAnswered = true;

        StatementData current = currentStatements[currentStatementIndex];
        bool isCorrect = (playerBelieves == current.isTrue);

        ProcessAnswer(isCorrect);
    }

    void ProcessAnswer(bool isCorrect)
    {
        StopAllCoroutines(); // Останавливаем таймер

        resultPanel.SetActive(true);

        StatementData current = currentStatements[currentStatementIndex];

        if (isCorrect)
        {
            feedbackText.text = "Правильно!";
            feedbackText.color = Color.green;
            correctAnswers++; // <-- Увеличиваем счётчик
        }
        else
        {
            feedbackText.text = "Неправильно!";
            feedbackText.color = Color.red;
        }

        explanationText.text = current.explanation;

        currentStatementIndex++;
    }

    void EndGame()
    {
        isGameActive = false;
        questionPanel.SetActive(false);
        resultPanel.SetActive(false);
        endPanel.SetActive(true);

        // Форматируем и отображаем результат
        resultText.text = $"Ваш результат: {correctAnswers} из {statements.Count}";
    }

    void RestartGame()
    {
        // Сбрасываем счётчик при перезапуске
        correctAnswers = 0;

        List<StatementData> shuffledStatements = new List<StatementData>(statements);
        ShuffleList(shuffledStatements);
        currentStatements = shuffledStatements;

        currentStatementIndex = 0;
        isGameActive = true;
        endPanel.SetActive(false);
        ShowNextStatement();
    }
}