using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class SceneButtonLoader : MonoBehaviour
{
    [Header("Scene to Load ")]
    [SerializeField] private string sceneName;

    [Header("Press Animation")]
    [Tooltip("Scale multiplier when button is pressed")]
    [SerializeField] private float pressedScale = 0.95f;
    [Tooltip("Duration of each scale step ")]
    [SerializeField] private float scaleDuration = 0.1f;

    [Header("Fade Transition")]
    [Tooltip("Delay before starting fade")]
    [SerializeField] private float delayBeforeFade = 0.2f;
    [Tooltip("Total duration of fade to black")]
    [SerializeField] private float fadeDuration = 0.7f;
    [Tooltip("Easing curve for fade alpha")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Button _button;
    private Vector3 _initialScale;
    private Image _fadeOverlay;

    private void Awake()
    {
   
        _button = GetComponent<Button>();
        _initialScale = transform.localScale;

  
        _button.onClick.AddListener(OnButtonPressed);

    
        CreateFadeOverlay();
    }

    private void OnButtonPressed()
    {
       
        StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
       
        yield return ScaleRoutine(_initialScale, _initialScale * pressedScale, scaleDuration);
        yield return ScaleRoutine(transform.localScale, _initialScale, scaleDuration);

       
        yield return new WaitForSeconds(delayBeforeFade);

       
        yield return FadeRoutine(0f, 1f, fadeDuration);

        
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ScaleRoutine(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.LerpUnclamped(from, to, elapsed / duration);
            yield return null;
        }
        transform.localScale = to;
    }

    private IEnumerator FadeRoutine(float fromAlpha, float toAlpha, float duration)
    {
        float elapsed = 0f;
        Color color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = fadeCurve.Evaluate(elapsed / duration);
            color = new Color(0f, 0f, 0f, Mathf.Lerp(fromAlpha, toAlpha, t));
            _fadeOverlay.color = color;
            yield return null;
        }
        _fadeOverlay.color = new Color(0f, 0f, 0f, toAlpha);
    }

    private void CreateFadeOverlay()
    {
     
        GameObject canvasGO = new GameObject("FadeCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

   
        GameObject imgGO = new GameObject("FadeOverlay");
        imgGO.transform.SetParent(canvasGO.transform, false);
        _fadeOverlay = imgGO.AddComponent<Image>();
        _fadeOverlay.color = new Color(0f, 0f, 0f, 0f);

        RectTransform rt = _fadeOverlay.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }
}
