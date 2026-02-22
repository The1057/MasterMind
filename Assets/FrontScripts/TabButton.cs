using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Этот скрипт вешаем на саму кнопку
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    public TabGroup tabGroup; // Ссылка на главный скрипт (заполни в инспекторе)
    public GameObject associatedElement; // Элемент (панель, картинка), который нужно показать (заполни в инспекторе)

    private Image buttonImage; // Компонент картинки кнопки
    private Button button; // Обычная кнопка (чтобы блокировать лишние нажатия)
    private Sprite defaultSprite; // Запоминаем изначальный спрайт

    void Start()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();

        if (buttonImage != null)
        {
            defaultSprite = buttonImage.sprite;
        }

        // Если группа еще не назначена, ищем её автоматически (на родителе или вручную)
        if (tabGroup == null)
            tabGroup = GetComponentInParent<TabGroup>();
    }

    // Этот метод вызовется при клике мышкой
    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && !button.interactable)
            return; // Не реагируем, если кнопка неактивна

        if (tabGroup != null)
        {
            tabGroup.OnTabSelected(this);
        }
    }

    // Методы для смены внешнего вида, которые будет вызывать TabGroup
    public void ActivateTab()
    {
        if (buttonImage != null && tabGroup != null && tabGroup.activeSprite != null)
        {
            buttonImage.sprite = tabGroup.activeSprite; // Меняем на спрайт активной кнопки
        }
        if (associatedElement != null)
            associatedElement.SetActive(true); // Показываем элемент
    }

    public void DeactivateTab()
    {
        if (buttonImage != null)
        {
            // Возвращаем обычный спрайт (или ставим неактивный, если есть)
            if (tabGroup != null && tabGroup.inactiveSprite != null)
                buttonImage.sprite = tabGroup.inactiveSprite;
            else
                buttonImage.sprite = defaultSprite; // Возвращаем исходный спрайт
        }

        if (associatedElement != null)
            associatedElement.SetActive(false); // Прячем элемент
    }
}