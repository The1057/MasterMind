using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class nazadmap : MonoBehaviour
{
    [Header("Настройки перехода")]
    [SerializeField] private string sceneName;
    [SerializeField] private int canvasIndexToSet = 0;

    [Header("Анимация появления (Лист)")]
    [Tooltip("Цвет вылетающего листа")]
    [SerializeField] private Color sheetColor = Color.black;
    [SerializeField] private float expandDuration = 0.4f;
    [SerializeField] private AnimationCurve expandCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Зависимости")]
    public saveLoadManager saveLoadManager;

    private Button _button;
    private RectTransform _buttonRect;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _buttonRect = GetComponent<RectTransform>(); //

        if (_button != null)
            _button.onClick.AddListener(OnButtonPressed); //
    }

    private void OnButtonPressed()
    {
        setTargetCanvas(canvasIndexToSet); //
        StartCoroutine(AnimateSheetAndLoad()); //
    }

    public void setTargetCanvas(int canvasIndex)
    {
        if (saveLoadManager != null)
        {
            var saveData = saveLoadManager.loadData(); //
            if (saveData != null)
            {
                saveData.targetCanvas = canvasIndex; //
                saveLoadManager.saveData(saveData); //
            }
        }
    }

    private IEnumerator AnimateSheetAndLoad()
    {
        GameObject canvasGO = new GameObject("TransitionCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGO.AddComponent<CanvasScaler>();

        GameObject sheetGO = new GameObject("TransitionSheet");
        sheetGO.transform.SetParent(canvasGO.transform, false);
        Image sheetImage = sheetGO.AddComponent<Image>();
        sheetImage.color = sheetColor;

        RectTransform rt = sheetImage.rectTransform;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);

        // ИСПРАВЛЕНО: используем .rect.size вместо .size
        Vector2 buttonSize = _buttonRect.rect.size; //
        rt.sizeDelta = buttonSize; //
        rt.position = _buttonRect.position; //

        float elapsed = 0f;
        Vector2 targetSize = new Vector2(Screen.width * 3f, Screen.height * 3f);
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 startPos = rt.position;

        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float t = expandCurve.Evaluate(elapsed / expandDuration); //

            rt.sizeDelta = Vector2.Lerp(buttonSize, targetSize, t); //
            rt.position = Vector3.Lerp(startPos, screenCenter, t); //

            yield return null;
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName); //
        }
    }
}