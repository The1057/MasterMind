//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public class TestManager3 : MonoBehaviour
//{
//    public GameObject questionUIPrefab;
//    public List<Question> questions;

//    // Этот список будет хранить созданные объекты, а не префабы
//    private List<GameObject> generatedQuestions = new List<GameObject>();

//    public int currentQuestionIndex = 0;

//    void Start()
//    {
//        GenerateAllQuestions();
//        SetQuestionByIndex(0);
//    }

//    void GenerateAllQuestions()
//    {
//        foreach (var qData in questions)
//        {
//            GameObject newQuestion = Instantiate(questionUIPrefab, transform);
//            // newQuestion.GetComponent<QuestionUI>().Setup(qData);

//            // Здесь нужна функция для заполнения
//            TextMeshProUGUI questionText = newQuestion.transform.Find("QuestionText").GetComponent<TextMeshProUGUI>();
//            questionText.text = qData.question;

//            // Здесь будет логика для создания кнопок ответов

//            generatedQuestions.Add(newQuestion);
//            newQuestion.SetActive(false); // Скрываем все вопросы
//        }
//    }

//    public void SetQuestionByIndex(int index)
//    {
//        // Скрываем предыдущий вопрос
//        if (currentQuestionIndex >= 0 && currentQuestionIndex < generatedQuestions.Count)
//        {
//            generatedQuestions[currentQuestionIndex].SetActive(false);
//        }

//        // Показываем новый
//        currentQuestionIndex = index;
//        generatedQuestions[currentQuestionIndex].SetActive(true);
//    }

//    // Метод для переключения на следующий вопрос
//    public void GoToNextQuestion()
//    {
//        SetQuestionByIndex(currentQuestionIndex + 1);
//    }
//}