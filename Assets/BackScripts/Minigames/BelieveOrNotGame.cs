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
    public TMP_Text questionCounterText;

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

    public int numberOfQuestionsToUse = 10;

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

        questionCounterText.text = $"{currentStatementIndex + 1}/{currentStatements.Count}";

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
    List<StatementData> GetRandomStatements(List<StatementData> sourceList, int count)
    {
        if (count <= 0 || sourceList.Count == 0)
        {
            return new List<StatementData>();
        }

        // Создаем копию списка, чтобы не изменять оригинальный
        List<StatementData> shuffled = new List<StatementData>(sourceList);
        ShuffleList(shuffled);

        // Возвращаем первые N элементов
        int actualCount = Mathf.Min(count, shuffled.Count);
        return shuffled.GetRange(0, actualCount);
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
        resultText.text = $"Ваш результат: {correctAnswers} из {currentStatements.Count}";
    }

    void RestartGame()
    {
        // Сбрасываем счётчик при перезапуске
        correctAnswers = 0;

        currentStatements = GetRandomStatements(statements, numberOfQuestionsToUse);

        currentStatementIndex = 0;
        isGameActive = true;
        endPanel.SetActive(false);
        ShowNextStatement();
    }
}