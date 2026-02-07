using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ArchivePlate : MonoBehaviour
{
    [Header("UI элементы (внутри префаба)")]
    public TMP_Text titleText;
    public Transform buttonSlotLeft;
    public Transform buttonSlotRight;

    [Header("Настройки этой плашки")]
    public string plateName;
    public Button actionButton1; // Перетаскиваешь кнопку со сцены (которая уже в маппинге CanvasSwitcher)
    public Button actionButton2;

    public void Initialize()
    {
        if (titleText != null) titleText.text = plateName;

        // Автоматически перемещаем кнопки в слоты префаба, если они назначены
        SetupButton(actionButton1, buttonSlotLeft);
        SetupButton(actionButton2, buttonSlotRight);
    }

    private void SetupButton(Button btn, Transform slot)
    {
        if (btn != null && slot != null)
        {
            RectTransform rt = btn.GetComponent<RectTransform>();
            btn.transform.SetParent(slot, false);

            // Растягиваем кнопку по размеру слота
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}