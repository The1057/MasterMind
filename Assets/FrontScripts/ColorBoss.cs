using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBoss : MonoBehaviour, ISaveLoadable
{
    [Header("Настройки тегов")]
    public string lightGroupTag = "LightGroup"; // Тег для светлых объектов
    public string darkGroupTag = "DarkGroup";   // Тег для тёмных объектов

    
    public Color lightColor;
    public Color darkColor;
    void Start()
    {
        
        string hex = "#76B5D7"; 
        if (ColorUtility.TryParseHtmlString(hex, out Color newColor))
        {
            lightColor = newColor;
        }
        string hex2 = "#0C041B";
        if (ColorUtility.TryParseHtmlString(hex2, out Color newColor2))
        {
            darkColor = newColor2;
        }
        FindObjectsByTag();
        ApplyColors();
    }
    

    // Внутренние списки, заполняются автоматически
    private List<GameObject> lightObjects = new List<GameObject>();
    private List<GameObject> darkObjects = new List<GameObject>();

    public static ColorBoss Instance { get; private set; }

    void Awake()
    {
        Instance = this;

    }

    // В методах SetLightColor/SetDarkColor добавь:
    public void SetLightColor(Color newColor)
    {
        lightColor = newColor;

        // Обновляем все существующие объекты в сцене
        foreach (GameObject obj in lightObjects)
        {
            if (obj != null)
            {
                Image img = obj.GetComponent<Image>();
                if (img != null)
                {
                    Color c = newColor;
                    c.a = img.color.a;
                    img.color = c;
                }
            }
        }

        // Обновляем все префабы через компонент
        PrefabColorController[] prefabs = FindObjectsByType<PrefabColorController>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        foreach (var prefab in prefabs)
        {
            if (prefab.colorTag == "LightGroup")
            {
                prefab.ForceUpdate(newColor);
            }
        }
    }
    

    // Находит все объекты по тегам и заполняет списки
    void FindObjectsByTag()
    {
        // Resources.FindObjectsOfTypeAll находит ВСЕ объекты (включая неактивные и префабы)
        Image[] allImages = Resources.FindObjectsOfTypeAll<Image>();

        lightObjects.Clear();
        darkObjects.Clear();

        foreach (Image img in allImages)
        {
            // Проверяем, принадлежит ли объект сцене (не префабу)
            if (img.gameObject.scene.isLoaded && img.gameObject.scene.name != null)
            {
                if (img.CompareTag(lightGroupTag))
                {
                    lightObjects.Add(img.gameObject);
                }
                else if (img.CompareTag(darkGroupTag))
                {
                    darkObjects.Add(img.gameObject);
                }
            }
        }

        Debug.Log($"Найдено ВСЕХ светлых: {lightObjects.Count}, тёмных: {darkObjects.Count}");
    }

    // Применяет текущие цвета ко всем объектам
    void ApplyColors()
    {
        foreach (GameObject obj in lightObjects)
        {
            if (obj != null)
            {
                Image img = obj.GetComponent<Image>();
                if (img != null)
                {
                    Color c = lightColor;
                    c.a = img.color.a; // сохраняем исходную прозрачность
                    img.color = c;
                }
            }
        }

        foreach (GameObject obj in darkObjects)
        {
            if (obj != null)
            {
                Image img = obj.GetComponent<Image>();
                if (img != null)
                {
                    Color c = darkColor;
                    c.a = img.color.a;
                    img.color = c;
                }
            }
        }
    }

    // Публичный метод для смены цвета из другого скрипта или UI
    

    public void SetDarkColor(Color newColor)
    {
        darkColor = newColor;
        // Обновляем все существующие объекты в сцене
        foreach (GameObject obj in darkObjects)
        {
            if (obj != null)
            {
                Image img = obj.GetComponent<Image>();
                if (img != null)
                {
                    Color c = newColor;
                    c.a = img.color.a;
                    img.color = c;
                }
            }
        }

        // Обновляем все префабы через компонент
        PrefabColorController[] prefabs = FindObjectsByType<PrefabColorController>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        foreach (var prefab in prefabs)
        {
            if (prefab.colorTag == "DarkGroup")
            {
                prefab.ForceUpdate(newColor);
            }
        }
    }

    // Метод для смены сразу обоих цветов
    public void SetBothColors(Color light, Color dark)
    {
        lightColor = light;
        darkColor = dark;
        ApplyColors();
    }

    // Опционально: можно обновить список объектов вручную (если объекты появляются позже)
    public void RefreshObjects()
    {
        FindObjectsByTag();
        ApplyColors();
    }

    public void save(ref saveData data)
    {
        data.colorBossData.lightColor = lightColor;
        data.colorBossData.darkColor = darkColor;
    }

    public void load(saveData data)
    {
        lightColor = data.colorBossData.lightColor;
        darkColor = data.colorBossData.darkColor;
        RefreshObjects();
    }
}