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

    private const float INITIAL_TIME = 10f;
    private const float RESTART_TIME = 5f;

    void Start()
    {
        // Подписываем кнопки
        believeButton.onClick.AddListener(() => OnAnswerSelected(true));
        notBelieveButton.onClick.AddListener(() => OnAnswerSelected(false));
        nextButton.onClick.AddListener(ShowNextStatement);
        restartButton.onClick.AddListener(RestartGame);

        RestartGame();
    }

    void RestartGame()
    {
        currentStatementIndex = 0;
        isGameActive = true;
        endPanel.SetActive(false);
        ShowNextStatement();
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

        StatementData current = statements[currentStatementIndex];
        statementText.text = current.statement;

        // Устанавливаем таймер
        timeLeft = isGameActive ? INITIAL_TIME : RESTART_TIME;
        StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        while (timeLeft > 0 && !isAnswered)
        {
            timerText.text = Mathf.Ceil(timeLeft).ToString() + "s";
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        if (!isAnswered)
        {
            // Время вышло — автоматически "неправильный" ответ
            ProcessAnswer(false);
        }
    }

    void OnAnswerSelected(bool playerBelieves)
    {
        if (isAnswered) return;
        isAnswered = true;

        StatementData current = statements[currentStatementIndex];
        bool isCorrect = (playerBelieves == current.isTrue);

        ProcessAnswer(isCorrect);
    }

    void ProcessAnswer(bool isCorrect)
    {
        StopAllCoroutines(); // Останавливаем таймер

        questionPanel.SetActive(false);
        resultPanel.SetActive(true);

        StatementData current = statements[currentStatementIndex];

        if (isCorrect)
        {
            feedbackText.text = "Правильно!";
            feedbackText.color = Color.green;
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
        endPanel.SetActive(true);
        questionPanel.SetActive(false);
        resultPanel.SetActive(false);
    }
}