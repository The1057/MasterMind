using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class Activator : MonoBehaviour
{
    [Header("Canvas Settings")]
    [SerializeField] public GameObject targetCanvas; // Канвас, который должен активироваться

    [Header("Sprite Settings")]
    [SerializeField] private Sprite defaultSprite;   // Спрайт по умолчанию
    [SerializeField] private Sprite activeSprite;    // Спрайт для активной иконки

    private Button button;
    private Image buttonImage;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        // Устанавливаем начальный спрайт
        if (defaultSprite != null)
        {
            buttonImage.sprite = defaultSprite;
        }

        // Подписываемся на клик
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        // Сначала обновляем все иконки — деактивируем другие
        UpdateAllIcons();

        // Активируем целевой канвас
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(true);
        }

        // Меняем свою иконку на активную
        if (activeSprite != null)
        {
            buttonImage.sprite = activeSprite;
        }
    }

    public void UpdateAllIcons()
    {
        // Находим все объекты с этим скриптом в сцене
        Activator[] allActivators = FindObjectsOfType<Activator>();

        foreach (Activator activator in allActivators)
        {
            if (activator == this) continue; // Пропускаем себя

            // У всех остальных ставим дефолтный спрайт
            if (activator.defaultSprite != null && activator.buttonImage != null)
            {
                activator.buttonImage.sprite = activator.defaultSprite;
            }

            // Деактивируем их канвасы (если не null)
            if (activator.targetCanvas != null && activator.targetCanvas != targetCanvas)
            {
                activator.targetCanvas.SetActive(false);
            }
        }
    }

    public void setIconToActive()
    {
        if (activeSprite != null)
        {
            buttonImage.sprite = activeSprite;
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}