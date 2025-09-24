using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class SkipScreensManager1 : MonoBehaviour
{
    public string dataFilePath = "PlayerData.json";

    public GameObject initialCanvas2; // например, Canvas_Intro или Canvas_Start


    // ВМЕСТО имён сцен — Canvas-объекты
    public GameObject maleProfileCanvas;   // например, Canvas_ProfileMan
    public GameObject femaleProfileCanvas; // например, Canvas_ProfileW
    public GameObject defaultStartCanvas;  // например, Canvas_Scene2

    public float fadeDuration = 1f;
    playerData playerData;
    [SerializeField] private Image fadeImage;

    void Awake()
    {
        if (fadeImage == null)
        {
            SetupFadeImage();
        }

        fadeImage.color = Color.black;
        StartCoroutine(FadeIn());
    }

    void Start()
    {
        string fullDataPath = GetFullPath(dataFilePath);

        bool dataExists = File.Exists(fullDataPath) && File.ReadAllText(fullDataPath).Trim() != "";

        GameObject targetCanvas = defaultStartCanvas; // по умолчанию

        if (dataExists)
        {
            string rawJSON;
            using (FileStream stream = new FileStream(fullDataPath, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                rawJSON = reader.ReadToEnd();
            }

            playerData = JsonUtility.FromJson<playerData>(rawJSON);

            if (playerData.player_gender == "F")
            {
                Debug.Log("Гендер Ж установлен. Показываем: " + femaleProfileCanvas.name);
                targetCanvas = femaleProfileCanvas;
            }
            else if (playerData.player_gender == "M")
            {
                Debug.Log("Гендер М установлен. Показываем: " + maleProfileCanvas.name);
                targetCanvas = maleProfileCanvas;
            }
            else if (!string.IsNullOrEmpty(playerData.player_gender))
            {
                Debug.LogWarning("Неизвестный гендер: " + playerData.player_gender + ". Показываем экран по умолчанию.");
            }
        }
        else
        {
            Debug.Log("Данных не найдено. Показываем: " + defaultStartCanvas.name);
        }

        StartCoroutine(SwitchToCanvasWithFade(targetCanvas));
    }

    private void SetupFadeImage()
    {
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = Color.black;

        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color startColor = Color.black;
        Color endColor = new Color(0f, 0f, 0f, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }

    IEnumerator SwitchToCanvasWithFade(GameObject targetCanvas)
    {

        // Показываем затемнение
        fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Color startColor = new Color(0f, 0f, 0f, 0f);
        Color endColor = Color.black;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        // Показываем нужный Canvas
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(true);
        }

        initialCanvas2.SetActive(false);
        fadeImage.gameObject.SetActive(false);
    }


    private string GetFullPath(string fileName)
    {
#if UNITY_EDITOR
        string scriptPath = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        return Path.Combine(scriptDirectory, fileName);
#else
        return Path.Combine(Application.persistentDataPath, fileName);
#endif
    }
}