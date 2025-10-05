using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static TestManager3;

public class QuestionUI : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public List<Button> answerButtons;
    public TextMeshProUGUI explanationText;
    public Button nextButton;

    // Спрайты и другие UI-элементы для ответа
    public Sprite defaultSprite;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    private int correctAnswerIndex;

    public void Setup(Question question, TestManager3 manager)
    {
        questionText.text = question.question;
        explanationText.text = question.comment;
        explanationText.color = Color.clear;

        // Очищаем старые обработчики, чтобы не было дублирования
        foreach (var button in answerButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        // Привязываем кнопки к методу ответа в менеджере
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.answers[i];
            int index = i;
            answerButtons[i].onClick.AddListener(() => manager.OnAnswerSelected(index));
        }

        correctAnswerIndex = question.correctAnswerIndex;
        nextButton.onClick.AddListener(manager.GoToNextUnansweredQuestion);
        nextButton.gameObject.SetActive(false);
    }

    public void DisplayCorrectness(int selectedIndex, bool isCorrect)
    {
        // Изменяем цвет кнопки, которую выбрал игрок
        answerButtons[selectedIndex].image.sprite = isCorrect ? correctSprite : wrongSprite;

        // Показываем правильный ответ
        if (!isCorrect)
        {
            answerButtons[correctAnswerIndex].image.sprite = correctSprite;
        }

        // Показываем пояснение
        explanationText.color = Color.white;

        // Отключаем кнопки после ответа
        foreach (var button in answerButtons)
        {
            button.interactable = false;
        }
    }

    public void UpdateUI(bool isAnswered)
    {
        // Сброс UI при переключении вопроса
        explanationText.color = Color.clear;
        foreach (var button in answerButtons)
        {
            button.image.sprite = defaultSprite;
            button.interactable = !isAnswered;
        }
        nextButton.gameObject.SetActive(false);
    }

    public void ShowNextButton()
    {
        nextButton.gameObject.SetActive(true);
    }
}