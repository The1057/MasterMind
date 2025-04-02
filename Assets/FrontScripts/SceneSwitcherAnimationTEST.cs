using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneSwitcherAnimation : MonoBehaviour
{
    public string nextSceneName; // Название следующей сцены
    public float fadeDuration = 1f; // Длительность анимации

    private static SceneSwitcherAnimation instance;
    private static float originalFixedDeltaTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            originalFixedDeltaTime = Time.fixedDeltaTime;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            StartCoroutine(FadeAndSwitchScene());
        }
    }

    IEnumerator FadeAndSwitchScene()
    {
        yield return StartCoroutine(FadeToBlack()); // Затемнение
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeFromBlack()); // Осветление
    }

    IEnumerator FadeToBlack()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            Time.timeScale = Mathf.Lerp(1, 0, t); // Плавное замедление времени
            Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
            yield return null;
        }
        Time.timeScale = 0;
    }

    IEnumerator FadeFromBlack()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            Time.timeScale = Mathf.Lerp(0, 1, t); // Плавное ускорение времени
            Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;
            yield return null;
        }
        Time.timeScale = 1;
    }
}
