using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    public string Scene2;
    public float delay = 1f;
    public float fadeDuration = 1f; 

    [SerializeField] private Image fadeImage; 

    void Start()
    {
       
        if (fadeImage == null)
        {
            SetupFadeImage();
        }

      
        fadeImage.color = Color.black;
        StartCoroutine(FadeInAndLoad());
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


        RectTransform rect = fadeImage.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    IEnumerator FadeInAndLoad()
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

   
        yield return new WaitForSeconds(delay);

       
        yield return StartCoroutine(FadeToWhite());

        SceneManager.LoadScene(Scene2);
    }

    IEnumerator FadeToWhite()
    {
        float elapsedTime = 0f;
        Color startColor = new Color(0f, 0f, 0f, 0f);
        Color endColor = Color.white;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    public void LoadNextScene()
    {
        StartCoroutine(FadeAndLoadNext());
    }

    IEnumerator FadeAndLoadNext()
    {
        yield return StartCoroutine(FadeToWhite());
        SceneManager.LoadScene(Scene2);
    }
}