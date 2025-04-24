
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TapToGraph : MonoBehaviour
{
    // Привязываем кнопку в инспекторе
    [SerializeField] private Button transitionButton;

    void Start()
    {
        // Проверяем, что кнопка привязана
        if (transitionButton != null)
        {
            // Добавляем слушатель события на нажатие кнопки
            transitionButton.onClick.AddListener(GoToAnalizzzScene);
        }
        else
        {
            Debug.LogError("Кнопка не привязана в инспекторе!");
        }
    }

    // Метод для перехода на сцену Analizzz
    void GoToAnalizzzScene()
    {
        SceneManager.LoadScene("Analizzz");
    }

    // Очистка слушателя при уничтожении объекта
    void OnDestroy()
    {
        if (transitionButton != null)
        {
            transitionButton.onClick.RemoveListener(GoToAnalizzzScene);
        }
    }
}