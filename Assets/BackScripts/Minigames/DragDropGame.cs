using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragDropGame : MonoBehaviour
{
    [Header("UI")]
    public GameObject cardPrefab;
    public Transform cardSpawnPoint;
    public DropZone oooZone; // Зона для ООО
    public DropZone ipZone;  // Зона для ИП
    public TMP_Text timerText; // Для отображения "Готово!" или можно убрать
    public Canvas gameCanvas; // Основной игровой Canvas
    public Canvas resultCanvas; // Ссылка на Canvas с результатами
    public TMP_Text resultText; // Ссылка на текст результата
    public Button restartButton; // Ссылка на кнопку рестарта

    [Header("Data")]
    public List<StatementData> statements;

    private List<StatementData> currentStatements;
    private int currentStatementIndex = 0;
    private DraggableCard currentCard;
    private bool gameEnded = false;
    private bool isProcessingFeedback = false; // Флаг для блокировки во время feedback
    private int correctAnswers = 0; // Счётчик правильных ответов
    private int totalQuestions = 0; // Общее количество вопросов
    private HashSet<int> incorrectlyAnswered = new HashSet<int>(); // Отслеживаем вопросы с неправильными ответами

    void Start()
    {
        // Назначение DropZone их категории
        if (oooZone != null) oooZone.SetExpectedForm(LegalForm.OOO);
        else Debug.LogError("oooZone is not assigned!");
        if (ipZone != null) ipZone.SetExpectedForm(LegalForm.IP);
        else Debug.LogError("ipZone is not assigned!");

        // Убедимся, что финальный экран скрыт, а игровой показан
        if (resultCanvas != null) resultCanvas.gameObject.SetActive(false);
        else Debug.LogError("resultCanvas is not assigned!");
        if (gameCanvas != null) gameCanvas.gameObject.SetActive(true);
        else Debug.LogError("gameCanvas is not assigned!");

        // Подключаем кнопку рестарта
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }
        else
        {
            Debug.LogError("restartButton is not assigned!");
        }

        StartNewGame();
    }

    public void StartNewGame()
    {
        // Сбрасываем счётчик и задаём общее количество вопросов
        correctAnswers = 0;
        totalQuestions = statements.Count;
        incorrectlyAnswered.Clear(); // Очищаем список вопросов с неправильными ответами

        // Создаем копию и перемешиваем
        currentStatements = new List<StatementData>(statements);
        ShuffleList(currentStatements);

        currentStatementIndex = 0;
        gameEnded = false;
        isProcessingFeedback = false;

        // Показываем игровой Canvas и скрываем финальный
        if (gameCanvas != null) gameCanvas.gameObject.SetActive(true);
        if (resultCanvas != null) resultCanvas.gameObject.SetActive(false);

        // Удаляем отображение таймера
        if (timerText != null) timerText.text = "";

        ShowNextCard();
    }

    void ShowNextCard()
    {
        if (currentStatementIndex >= currentStatements.Count)
        {
            EndGame();
            return;
        }

        // Удаляем старую карточку, если есть
        if (currentCard != null && currentCard.gameObject != null)
        {
            Destroy(currentCard.gameObject);
        }

        // Создаём новую
        if (cardPrefab == null || cardSpawnPoint == null)
        {
            Debug.LogError("cardPrefab or cardSpawnPoint is not assigned!");
            return;
        }

        GameObject cardObj = Instantiate(cardPrefab, cardSpawnPoint);
        currentCard = cardObj.GetComponent<DraggableCard>();
        if (currentCard == null)
        {
            Debug.LogError("DraggableCard component not found on instantiated card!");
            Destroy(cardObj);
            return;
        }
        currentCard.Initialize(this, currentStatements[currentStatementIndex]);

        // Сбрасываем цвета зон
        if (oooZone != null) oooZone.ResetColor();
        if (ipZone != null) ipZone.ResetColor();

        isProcessingFeedback = false; // Разрешаем взаимодействие
        Debug.Log($"Showing card {currentStatementIndex + 1}/{totalQuestions}");
    }

    public bool OnCardDropped(DraggableCard card, DropZone zone)
    {
        if (card != currentCard || gameEnded || isProcessingFeedback)
        {
            Debug.LogWarning($"OnCardDropped ignored: Invalid card={card == currentCard}, gameEnded={gameEnded}, isProcessingFeedback={isProcessingFeedback}");
            return false;
        }

        if (zone == null)
        {
            Debug.LogError("DropZone is null in OnCardDropped!");
            return false;
        }

        isProcessingFeedback = true; // Блокируем дальнейшие дропы до завершения обработки
        Debug.Log($"OnCardDropped called for zone {zone.gameObject.name}");

        StatementData current = currentStatements[currentStatementIndex];
        bool isCorrect = (zone.ExpectedForm == current.correctForm);

        if (isCorrect)
        {
            OnCorrectDrop(zone);
        }
        else
        {
            OnWrongDrop(zone);
        }
        return true;
    }

    public void SetProcessingFeedback(bool value)
    {
        isProcessingFeedback = value;
        Debug.Log($"SetProcessingFeedback: {value}");
    }

    void OnCorrectDrop(DropZone droppedZone)
    {
        // Засчитываем правильный ответ только если вопроса нет в incorrectlyAnswered
        if (!incorrectlyAnswered.Contains(currentStatementIndex))
        {
            correctAnswers++;
            Debug.Log($"Correct answer! Score: {correctAnswers}/{totalQuestions}");
        }

        if (oooZone != null) oooZone.ResetColor();
        if (ipZone != null) ipZone.ResetColor();

        // Подсвечиваем правильную зону зеленым
        StartCoroutine(droppedZone.FlashColor(Color.green));

        // Расчет целевой позиции для притяжения
        RectTransform zoneRect = droppedZone.GetComponent<RectTransform>();
        Vector2 targetPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cardSpawnPoint.GetComponent<RectTransform>(),
            zoneRect.position,
            null,
            out targetPosition);

        // Притягиваем карточку, затем запускаем исчезновение
        currentCard.SnapToPosition(targetPosition, () => StartCoroutine(FadeOutCardAndNext()));
    }

    IEnumerator FadeOutCardAndNext()
    {
        // Плавное исчезновение
        CanvasGroup cg = currentCard.GetComponent<CanvasGroup>();
        if (cg == null) cg = currentCard.gameObject.AddComponent<CanvasGroup>();

        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = 1f - (elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0f;

        // Переход к следующему вопросу
        currentStatementIndex++;
        yield return new WaitForSeconds(0.3f);

        ShowNextCard();
    }

    void OnWrongDrop(DropZone droppedZone)
    {
        // Добавляем индекс вопроса в список неправильных ответов
        incorrectlyAnswered.Add(currentStatementIndex);
        Debug.Log($"Wrong answer on question {currentStatementIndex}");

        // Определяем правильную зону для обратной связи
        StatementData current = currentStatements[currentStatementIndex];
        DropZone correctZone = (current.correctForm == LegalForm.OOO) ? oooZone : ipZone;

        // Подсвечиваем зоны
        StartCoroutine(droppedZone.FlashColor(Color.red));
        if (correctZone != null)
        {
            StartCoroutine(correctZone.FlashColor(Color.green));
        }
        else
        {
            Debug.LogError("Correct DropZone is null!");
        }

        // Возвращаем карточку
        currentCard.ReturnToStart();
    }

    void EndGame()
    {
        gameEnded = true;
        isProcessingFeedback = true; // Блокируем взаимодействие
        if (timerText != null) timerText.text = "Готово!";
        if (currentCard != null && currentCard.gameObject != null)
        {
            Destroy(currentCard.gameObject);
        }

        // Скрываем игровой Canvas и показываем финальный
        if (gameCanvas != null)
        {
            gameCanvas.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("gameCanvas is not assigned in EndGame!");
        }

        if (resultCanvas != null)
        {
            resultCanvas.gameObject.SetActive(true);
            if (resultText != null)
            {
                resultText.text = $"Результат: {correctAnswers}/{totalQuestions}";
            }
        }
        else
        {
            Debug.LogError("resultCanvas is not assigned in EndGame!");
        }
    }

    void OnRestartButtonClicked()
    {
        // Скрываем финальный экран и показываем игровой
        if (resultCanvas != null) resultCanvas.gameObject.SetActive(false);
        if (gameCanvas != null) gameCanvas.gameObject.SetActive(true);

        // Запускаем новую игру
        StartNewGame();
    }

    // Fisher-Yates shuffle
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
}