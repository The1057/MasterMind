using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class DragDropGame : MonoBehaviour
{
    [Header("UI")]
    public GameObject cardPrefab;
    public Transform cardSpawnPoint;
    public List<DropZone> dropZones;
    public Canvas gameCanvas;
    public Canvas resultCanvas;
    public TMP_Text resultText;
    public Button restartButton;

    [Header("Объяснение ошибки")]
    public GameObject explanationPanel;
    public TMP_Text explanationText;

    [Header("Результат")]
    public int winThreshold = 7;
    [TextArea] public string winMessage = "Отлично! Ты победил!";
    [TextArea] public string loseMessage = "Не хватило правильных ответов. Попробуй ещё раз!";

    [Header("События")]
    public UnityEvent onWin;       // каждый раз при победе
    public UnityEvent onLose;      // каждый раз при поражении
    public UnityEvent onFirstWin;  // только при самой первой победе (сюда алмазики)

    [Header("Data")]
    public List<StatementData> statements;

    private const string FirstWinKey = "DragDropGame_FirstWinDone";

    private List<StatementData> currentStatements;
    private int currentStatementIndex;
    private DraggableCard currentCard;
    private bool gameEnded;
    private bool isProcessingFeedback;
    private int correctAnswers;
    private int totalQuestions;
    private readonly HashSet<int> incorrectlyAnswered = new HashSet<int>();

    void Start()
    {
        restartButton.onClick.AddListener(StartNewGame);
        StartNewGame();
    }

    public void StartNewGame()
    {
        correctAnswers = 0;
        totalQuestions = statements.Count;
        incorrectlyAnswered.Clear();

        currentStatements = new List<StatementData>(statements);
        ShuffleList(currentStatements);

        currentStatementIndex = 0;
        gameEnded = false;
        isProcessingFeedback = false;

        gameCanvas.gameObject.SetActive(true);
        resultCanvas.gameObject.SetActive(false);

        ShowNextCard();
    }

    void ShowNextCard()
    {
        if (currentStatementIndex >= currentStatements.Count)
        {
            EndGame();
            return;
        }

        if (currentCard != null)
            Destroy(currentCard.gameObject);

        GameObject cardObj = Instantiate(cardPrefab, cardSpawnPoint);
        currentCard = cardObj.GetComponent<DraggableCard>();
        currentCard.Initialize(this, currentStatements[currentStatementIndex]);

        ResetZoneColors();
        explanationPanel.SetActive(false);

        isProcessingFeedback = false;
    }

    public bool OnCardDropped(DraggableCard card, DropZone zone)
    {
        if (card != currentCard || gameEnded || isProcessingFeedback)
            return false;

        isProcessingFeedback = true;

        StatementData current = currentStatements[currentStatementIndex];

        if (zone.ZoneId == current.correctZoneId)
            OnCorrectDrop(zone);
        else
            OnWrongDrop(zone, current);

        return true;
    }

    public void SetProcessingFeedback(bool value)
    {
        isProcessingFeedback = value;
    }

    void OnCorrectDrop(DropZone droppedZone)
    {
        if (!incorrectlyAnswered.Contains(currentStatementIndex))
            correctAnswers++;

        ResetZoneColors();
        explanationPanel.SetActive(false);
        StartCoroutine(droppedZone.FlashColor(Color.green));

        RectTransform zoneRect = droppedZone.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardSpawnPoint.GetComponent<RectTransform>(),
            zoneRect.position,
            null,
            out Vector2 targetPosition);

        currentCard.SnapToPosition(targetPosition, () => StartCoroutine(FadeOutCardAndNext()));
    }

    void OnWrongDrop(DropZone droppedZone, StatementData current)
    {
        incorrectlyAnswered.Add(currentStatementIndex);

        StartCoroutine(droppedZone.FlashColor(Color.red));
        DropZone correctZone = dropZones.Find(z => z.ZoneId == current.correctZoneId);
        StartCoroutine(correctZone.FlashColor(Color.green));

        explanationText.text = current.explanation;
        explanationPanel.SetActive(true);

        currentCard.ReturnToStart();
    }

    IEnumerator FadeOutCardAndNext()
    {
        CanvasGroup cg = currentCard.GetComponent<CanvasGroup>();
        if (cg == null) cg = currentCard.gameObject.AddComponent<CanvasGroup>();

        const float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = 1f - elapsed / duration;
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0f;

        currentStatementIndex++;
        yield return new WaitForSeconds(0.3f);

        ShowNextCard();
    }

    void EndGame()
    {
        gameEnded = true;
        isProcessingFeedback = true;

        if (currentCard != null)
            Destroy(currentCard.gameObject);

        gameCanvas.gameObject.SetActive(false);
        resultCanvas.gameObject.SetActive(true);

        bool isWin = correctAnswers >= winThreshold;
        resultText.text = $"{(isWin ? winMessage : loseMessage)}\nРезультат: {correctAnswers}/{totalQuestions}";

        if (isWin)
        {
            onWin?.Invoke();

            if (PlayerPrefs.GetInt(FirstWinKey, 0) == 0)
            {
                PlayerPrefs.SetInt(FirstWinKey, 1);
                PlayerPrefs.Save();
                onFirstWin?.Invoke();
            }
        }
        else
        {
            onLose?.Invoke();
        }
    }

    void ResetZoneColors()
    {
        foreach (DropZone zone in dropZones)
            zone.ResetColor();
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}