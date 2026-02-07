using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBoss: MonoBehaviour
{
    [SerializeField] private List<GameObject> targetObjects = new List<GameObject>();

    // Метод для изменения цвета всех объектов в списке
    public void ChangeColor(string colorName)
    {
        Color newColor;

        // Определяем цвет по строке (без учета регистра)
        switch (colorName.ToLower())
        {
            case "красный":
            case "red":
                newColor = new Color(0.9882354f, 0.5254902f, 0.9254903f);
                break;

            case "зеленый":
            case "green":
                newColor = new Color(0.23f, 1, 0);
                break;

            case "синий":
            case "blue":
                newColor = new Color(0.5411765f, 0.8274511f, 1);
                break;

            default:
                Debug.LogWarning($"Цвет '{colorName}' не распознан. Использую белый.");
                newColor = Color.white;
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
                    image.color = newColor;
                }
            }
        }

        Debug.Log($"Цвет изменен на: {colorName}");
    }
}
