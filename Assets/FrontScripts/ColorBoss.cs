using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBoss: MonoBehaviour
{
    [SerializeField] 
    public List<GameObject> targetObjects = new List<GameObject>();
    public List<GameObject> targetObjectsDark = new List<GameObject>();

    // Метод для изменения цвета всех объектов в списке
    public void ChangeColor(string colorName)
    {
        Color newColor;
        Color newColorD;
        // Определяем цвет по строке (без учета регистра)
        switch (colorName.ToLower())
        {
            case "красный":
            case "red":
                newColor = new Color(0.9882354f, 0.5254902f, 0.9254903f);
                newColorD = new Color(0.2117647f, 0, 0.1784213f);
                break;

            case "зеленый":
            case "green":
                newColor = new Color(0.23f, 1, 0); 
                newColorD = new Color(0, 0.1547169f, 0);
                break;

            case "синий":
            case "blue":
                newColor = new Color(0.5411765f, 0.8274511f, 1);
                newColorD = new Color(0.12f, 0.1f, 0.23f);
                break;

            default:
                Debug.LogWarning($"Цвет '{colorName}' не распознан. Использую белый.");
                newColor = Color.white;
                newColorD = Color.gray;
                break;
        }

        // Применяем цвет ко всем объектам в списке
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null)
            {
                Image image = obj.GetComponent<Image>();
                if (image != null)
                {
                    // Сохраняем текущую прозрачность
                    newColor.a = image.color.a;
                    image.color = newColor;
                }
            }
        }

        foreach (GameObject objD in targetObjectsDark)
        {
            if (objD != null)
            {
                Image imageD = objD.GetComponent<Image>();
                if (imageD != null)
                {
                    // Сохраняем текущую прозрачность
                    newColorD.a = imageD.color.a;
                    imageD.color = newColorD;
                }
            }
        }
    }
}
