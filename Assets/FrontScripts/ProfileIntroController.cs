using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Контролирует однократный показ последовательных объектов при первом открытии экрана профиля.
/// Работает независимо для каждого экземпляра.
/// </summary>
[DisallowMultipleComponent]
public class ProfileIntroController : MonoBehaviour
{
    [Header("Intro Objects")]
    [Tooltip("Список GameObject'ов на сцене, которые последовательно активируются при первом открытии экрана профиля")]
    public GameObject[] introObjects;

    [Header("Persistence")]
    [Tooltip("Если true — интро покажется один раз за всё время (сохраняется в PlayerPrefs). Иначе — один раз за сессию.")]
    public bool persistent = true;

    [Tooltip("Уникальный идентификатор для этого интро (используется в PlayerPrefs). Если пусто — используется имя объекта.")]
    public string uniqueId = "";

    private int _currentIndex;
    private bool _isAnimating;
    private bool _sessionPlayed; // теперь НЕ статический — свой для каждого экземпляра

    private string PlayerPrefsKey => "ProfileIntroPlayed_" + (!string.IsNullOrEmpty(uniqueId) ? uniqueId : name);

    void OnEnable()
    {
        // Проверяем, проигрывалось ли уже
        bool hasPlayed = persistent
            ? PlayerPrefs.GetInt(PlayerPrefsKey, 0) == 1
            : _sessionPlayed;

        if (!hasPlayed && introObjects != null && introObjects.Length > 0)
        {
            _isAnimating = true;
            _currentIndex = 0;

            // Деактивируем все, кроме первого
            for (int i = 0; i < introObjects.Length; i++)
            {
                introObjects[i].SetActive(i == 0);
            }
        }
        else
        {
            // Уже показывали — скрываем всё
            if (introObjects != null)
            {
                foreach (var obj in introObjects)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
            }
            _isAnimating = false;
        }
    }

    void Update()
    {
        if (!_isAnimating || introObjects == null || introObjects.Length == 0)
            return;

        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            // Поддержка тача на мобильных
            if (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Began)
                return;

            // Скрываем текущий
            introObjects[_currentIndex].SetActive(false);
            _currentIndex++;

            if (_currentIndex < introObjects.Length)
            {
                // Показываем следующий
                introObjects[_currentIndex].SetActive(true);
            }
            else
            {
                // Завершение
                _isAnimating = false;

                if (persistent)
                {
                    PlayerPrefs.SetInt(PlayerPrefsKey, 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    _sessionPlayed = true;
                }

                // Можно отключить компонент — он больше не нужен
                enabled = false;
            }
        }
    }
}