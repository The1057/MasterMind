using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class SkipScreensManager : MonoBehaviour
{
    public string dataFilePath = "PlayerData.json";
    public string maleProfileSceneName = "ProfileMan";
    public string femaleProfileSceneName = "ProfileW";
    public string defaultStartScene = "Scene2";
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

        if (dataExists)
        {
            if (File.Exists(fullDataPath))
            {
                string rawJSON;
                using (FileStream stream = new FileStream(fullDataPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        rawJSON = reader.ReadToEnd();//magic to read from file
                    }
                }
                playerData = JsonUtility.FromJson<playerData>(rawJSON);

                if (playerData.player_gender == "F")
                {
                    Debug.Log("Гендер Ж установлен. Переход к: " + femaleProfileSceneName);
                    StartCoroutine(LoadSceneWithFade(femaleProfileSceneName));
                    return;
                }
                else if (playerData.player_gender == "M")
                {
                    Debug.Log("Гендер М установлен. Переход к: " + maleProfileSceneName);
                    StartCoroutine(LoadSceneWithFade(maleProfileSceneName));
                    return;
                }
                else if (playerData.player_gender != "")
                {
                    Debug.LogWarning("Неизвестный гендер: " + playerData.player_name + ". Загрузка сцены по умолчанию.");
                    StartCoroutine(LoadSceneWithFade(defaultStartScene));
                    return;
                }
            }
        }
        else
        {
            Debug.Log("Данных не найдено. Загрузка: " + defaultStartScene);
            StartCoroutine(LoadSceneWithFade(defaultStartScene));
        }
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

    IEnumerator LoadSceneWithFade(string sceneName)
    {
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

        SceneManager.LoadScene(sceneName);
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