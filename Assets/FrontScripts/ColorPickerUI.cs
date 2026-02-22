using UnityEngine;
using UnityEngine.UI;

public class ColorPickerUI : MonoBehaviour
{
    public ColorBoss colorBoss;           // ссылка на главный скрипт
    public Slider sliderR, sliderG, sliderB; // слайдеры
    public Image previewImage;             // картинка для предпросмотра цвета
    public bool controlLightGroup = true;  // если true – светлая группа, false – тёмная
    public bool applyInRealTime = true;    // применять цвет сразу при движении слайдера

    void Start()
    {
        // Устанавливаем начальные значения слайдеров из текущего цвета
        Color current = controlLightGroup ? colorBoss.lightColor : colorBoss.darkColor;
        sliderR.value = current.r;
        sliderG.value = current.g;
        sliderB.value = current.b;

        // Обновляем превью при старте
        UpdatePreview();

        // Подписываемся на событие изменения слайдеров
        sliderR.onValueChanged.AddListener(delegate { OnSliderChanged(); });
        sliderG.onValueChanged.AddListener(delegate { OnSliderChanged(); });
        sliderB.onValueChanged.AddListener(delegate { OnSliderChanged(); });
    }

    // Вызывается при любом изменении слайдера
    void OnSliderChanged()
    {
        // Обновляем превью
        UpdatePreview();

        // Если включен режим реального времени - сразу применяем цвет
        if (applyInRealTime)
        {
            ApplyColor();
        }
    }

    // Обновляем цвет превью
    void UpdatePreview()
    {
        if (previewImage != null)
        {
            Color previewColor = new Color(sliderR.value, sliderG.value, sliderB.value);
            previewImage.color = previewColor;
        }
    }

    // Применяем цвет к объектам
    public void ApplyColor()
    {
        Color newColor = new Color(sliderR.value, sliderG.value, sliderB.value);
        if (controlLightGroup)
            colorBoss.SetLightColor(newColor);
        else
            colorBoss.SetDarkColor(newColor);
    }
}