using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Этот скрипт вешаем на пустой объект (родитель всех кнопок)
public class TabGroup : MonoBehaviour
{
    public Sprite activeSprite; // Спрайт для активной кнопки (заполни в инспекторе)
    public Sprite inactiveSprite; // Спрайт для неактивной кнопки (заполни в инспекторе)

    [SerializeField] private List<TabButton> tabButtons; // Список всех кнопок

    private TabButton selectedTab; // Текущая выбранная кнопка

    void Start()
    {
        // Автоматически находим все кнопки в детях, если список пуст
        if (tabButtons == null || tabButtons.Count == 0)
        {
            tabButtons = new List<TabButton>(GetComponentsInChildren<TabButton>());
        }

        // Проверяем, есть ли уже активная кнопка по умолчанию
        bool hasActive = false;
        foreach (TabButton tab in tabButtons)
        {
            if (tab.associatedElement != null && tab.associatedElement.activeSelf)
            {
                SelectTab(tab);
                hasActive = true;
                break;
            }
        }

        // Если ни одна не активна, активируем первую
        if (!hasActive && tabButtons.Count > 0)
        {
            SelectTab(tabButtons[0]);
        }
    }

    // Вызываем из кнопки при клике
    public void OnTabSelected(TabButton clickedTab)
    {
        SelectTab(clickedTab);
    }

    private void SelectTab(TabButton newTab)
    {
        if (selectedTab == newTab)
            return; // Если нажали на ту же самую кнопку, ничего не делаем

        // Деактивируем старую кнопку
        if (selectedTab != null)
        {
            selectedTab.DeactivateTab();
        }

        // Активируем новую
        selectedTab = newTab;
        selectedTab.ActivateTab();
    }

    // Метод для ручной установки стартовой вкладки (опционально)
    public void SetStartingTab(TabButton startingTab)
    {
        if (startingTab != null)
        {
            SelectTab(startingTab);
        }
    }
}