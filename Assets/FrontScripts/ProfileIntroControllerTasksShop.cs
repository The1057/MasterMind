using UnityEngine;
using System.Collections;

/// <summary>
/// Контролирует однократный показ последовательных объектов при первом открытии экрана профиля.
/// </summary>
[DisallowMultipleComponent]
public class ProfileIntroControllerTasksShop : MonoBehaviour
{
    [Header("Intro Objects")]
    [Tooltip("Список GameObject'ов на сцене, которые последовательно активируются при первом открытии экрана профиля")]
    public GameObject[] introObjects;

    private int _currentIndex;
    private bool _isAnimating;

    // Выберите нужное поведение: 
    // true  — один раз за всю жизнь приложения (сохраняется в PlayerPrefs);
    // false — один раз за сессию (при перезапуске приложения снова покажется).
    [SerializeField]
    private bool persistent = true;

    private const string PREF_KEY = "ProfileIntroPlayed";
    // Эта статическая переменная живёт до конца сессии
    private static bool _sessionPlayed = false;

    void OnEnable()
    {
        bool hasPlayed = persistent
            ? (PlayerPrefs.GetInt(PREF_KEY, 0) == 1)
            : _sessionPlayed;

        if (!hasPlayed && introObjects != null && introObjects.Length > 0)
        {
            // Начинаем анимацию
            _isAnimating = true;
            _currentIndex = 0;
            // Деактивируем все, активируем только первый
            for (int i = 0; i < introObjects.Length; i++)
                introObjects[i].SetActive(i == 0);
        }
        else
        {
            // Уже показывали — просто скрываем всё и отключаем этот компонент
            _isAnimating = false;
            if (introObjects != null)
                foreach (var obj in introObjects)
                    obj.SetActive(false);

            // Больше не нужен
            enabled = false;
        }
    }

    void Update()
    {
        if (!_isAnimating || introObjects == null || introObjects.Length == 0)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // Скрываем текущий и переходим к следующему
            introObjects[_currentIndex].SetActive(false);
            _currentIndex++;

            if (_currentIndex < introObjects.Length)
            {
                introObjects[_currentIndex].SetActive(true);
            }
            else
            {
                // Конец анимации — сохраняем флаг и отключаем компонент
                _isAnimating = false;

                if (persistent)
                {
                    PlayerPrefs.SetInt(PREF_KEY, 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    _sessionPlayed = true;
                }

                enabled = false;
            }
        }
    }
}
