using UnityEngine;
using UnityEngine.UI;

public class PrefabColorController : MonoBehaviour
{
    public string colorTag = "LightGroup"; // тег для этого префаба

    private Image[] images;

    void Awake()
    {
        // Запоминаем все Image компоненты в префабе
        images = GetComponentsInChildren<Image>(true);
    }

    void Start()
    {
        // При создании применяем текущий цвет из ColorBoss
        UpdateColor();
    }

    void OnEnable()
    {
        // Если префаб переиспользуется (пулинг объектов)
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (ColorBoss.Instance == null) return;

        Color targetColor;
        if (colorTag == "LightGroup")
            targetColor = ColorBoss.Instance.lightColor;
        else if (colorTag == "DarkGroup")
            targetColor = ColorBoss.Instance.darkColor;
        else
            return;

        foreach (Image img in images)
        {
            if (img != null)
            {
                Color c = targetColor;
                c.a = img.color.a;
                img.color = c;
            }
        }
    }

    // Метод для принудительного обновления (вызывается из ColorBoss)
    public void ForceUpdate(Color newColor)
    {
        foreach (Image img in images)
        {
            if (img != null)
            {
                Color c = newColor;
                c.a = img.color.a;
                img.color = c;
            }
        }
    }
}