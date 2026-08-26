using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditTableUI : MonoBehaviour
{
    // Структура для хранения строк таблицы
    [System.Serializable]
    public struct Row
    {
        public string title;
        [TextArea(2, 5)] public string description;
    }

    // Данные таблицы
    public Row[] rows =
    {
        new Row { title = "Потребительский", description = "Для физических лиц. Выдается на товары, услуги, личные нужды. Часто без залога. Ставки: 17–25% годовых." },
        new Row { title = "Бизнес-кредит", description = "Для ИП и юр.лиц: на развитие, закупки, оборот. Возможна господдержка. Ставка 12–18%." },
        new Row { title = "Ипотека", description = "Целевой кредит на жилье. В 2025 возможна господдержка (ставка 6% на новостройки)." },
        new Row { title = "Автокредит", description = "Кредит на покупку автомобиля. Есть льготные программы на отечественные авто." },
        new Row { title = "Овердрафт", description = "Кредитный лимит на счёте компании. Для покрытия кассовых разрывов." },
        new Row { title = "Кредитная карта", description = "Займ с лимитом. Льготный период до 100 дней. В 2025 — ставка до 29%." },
        new Row { title = "Микрокредит", description = "Быстрое займствование от МФО. Ставки 30%+. Только в крайнем случае." },
        new Row { title = "Лизинг", description = "Альтернатива кредиту: аренда техники/авто с выкупом." }
    };

    void Start()
    {
        // === Canvas ===
        var canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var c = canvas.GetComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // === Главная панель ===
        var panel = new GameObject("TablePanel", typeof(Image), typeof(VerticalLayoutGroup));
        panel.transform.SetParent(canvas.transform, false);
        var panelImg = panel.GetComponent<Image>();
        panelImg.color = new Color(0.85f, 1f, 0.85f); // светло-зелёный фон

        var layout = panel.GetComponent<VerticalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = true;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.spacing = 5;
        layout.padding = new RectOffset(20, 20, 20, 20);

        // === Генерация строк ===
        foreach (var row in rows)
        {
            var rowObj = new GameObject(row.title, typeof(Image), typeof(HorizontalLayoutGroup));
            rowObj.transform.SetParent(panel.transform, false);
            rowObj.GetComponent<Image>().color = Color.white;

            var rowLayout = rowObj.GetComponent<HorizontalLayoutGroup>();
            rowLayout.childForceExpandWidth = true;
            rowLayout.spacing = 10;

            // Первый столбец — заголовок
            var titleObj = new GameObject("Title", typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(rowObj.transform, false);
            var title = titleObj.GetComponent<TextMeshProUGUI>();
            title.text = row.title;
            title.fontSize = 24;
            title.alignment = TextAlignmentOptions.MidlineLeft;
            title.color = new Color(0f, 0.3f, 0f);
            title.enableWordWrapping = true;
            var titleRect = title.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(400, 100);

            // Второй столбец — описание
            var descObj = new GameObject("Description", typeof(TextMeshProUGUI));
            descObj.transform.SetParent(rowObj.transform, false);
            var desc = descObj.GetComponent<TextMeshProUGUI>();
            desc.text = row.description;
            desc.fontSize = 20;
            desc.alignment = TextAlignmentOptions.MidlineLeft;
            desc.enableWordWrapping = true;
            desc.color = Color.black;
            var descRect = desc.GetComponent<RectTransform>();
            descRect.sizeDelta = new Vector2(1000, 100);
        }
    }
}
