
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class ProfileIntroController : MonoBehaviour
{
    [Header("Объекты подсказок")]
    [SerializeField] private GameObject[] introObjects;

    [Header("Настройки сохранения")]
    [SerializeField] private bool persistent = true;

    private const string PREF_KEY = "ProfileIntroPlayed";

    private static bool _sessionPlayed;

    private int _currentIndex;
    private bool _isAnimating;

    private RectTransform _currentRect;
    private Canvas _currentCanvas;

    private GameObject _blockerObject;
    private Image _blockerImage;

    private void OnEnable()
    {
        _isAnimating = false;

        bool hasPlayed = persistent
            ? PlayerPrefs.GetInt(PREF_KEY, 0) == 1
            : _sessionPlayed;

        HideAllHints();

        if (hasPlayed)
        {
            enabled = false;
            return;
        }

        if (introObjects == null || introObjects.Length == 0)
        {
            Debug.LogError("ProfileIntroController: Нет подсказок!");
            enabled = false;
            return;
        }

        bool hasValidObject = false;

        foreach (GameObject obj in introObjects)
        {
            if (obj != null)
            {
                hasValidObject = true;
                break;
            }
        }

        if (!hasValidObject)
        {
            enabled = false;
            return;
        }

        // Сохраняем факт запуска
        if (persistent)
        {
            PlayerPrefs.SetInt(PREF_KEY, 1);
            PlayerPrefs.Save();
        }
        else
        {
            _sessionPlayed = true;
        }

        // Создаём блокировку интерфейса
        CreateInputBlocker();

        _currentIndex = 0;
        _isAnimating = true;

        ShowNextHint();
    }

    private void CreateInputBlocker()
    {
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("ProfileIntroController: Canvas не найден!");
            return;
        }

        _blockerObject = new GameObject(
            "ProfileIntro_InputBlocker",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        _blockerObject.transform.SetParent(canvas.transform, false);

        RectTransform rect = _blockerObject.GetComponent<RectTransform>();

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        _blockerImage = _blockerObject.GetComponent<Image>();

        // Полностью прозрачный блокировщик
        _blockerImage.color = new Color(0, 0, 0, 0);
        _blockerImage.raycastTarget = true;

        // Блокировщик должен находиться выше обычных элементов
        _blockerObject.transform.SetAsLastSibling();
    }

    private void Update()
    {
        if (!_isAnimating || _currentRect == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            CheckClick(Input.mousePosition);
        }
        else if (Input.touchCount > 0 &&
                 Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckClick(Input.GetTouch(0).position);
        }
    }

    private void ShowNextHint()
    {
        while (_currentIndex < introObjects.Length &&
               introObjects[_currentIndex] == null)
        {
            _currentIndex++;
        }

        if (_currentIndex >= introObjects.Length)
        {
            FinishIntro();
            return;
        }

        GameObject currentObject = introObjects[_currentIndex];

        currentObject.SetActive(true);

        // Подсказка должна быть выше блокировщика
        currentObject.transform.SetAsLastSibling();

        _currentRect = currentObject.GetComponent<RectTransform>();
        _currentCanvas = currentObject.GetComponentInParent<Canvas>();

        if (_currentRect == null)
        {
            Debug.LogError("У подсказки отсутствует RectTransform: " +
                           currentObject.name);

            currentObject.SetActive(false);
            _currentIndex++;
            ShowNextHint();
            return;
        }

        // Поднимаем блокировщик в иерархии перед подсказкой,
        // если подсказка находится в том же Canvas.
        if (_blockerObject != null &&
            _blockerObject.transform.parent == currentObject.transform.parent)
        {
            _blockerObject.transform.SetSiblingIndex(
                currentObject.transform.GetSiblingIndex()
            );
        }
    }

    private void CheckClick(Vector2 position)
    {
        if (_currentRect == null)
            return;

        Camera cam = null;

        if (_currentCanvas != null &&
            _currentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = _currentCanvas.worldCamera;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(
            _currentRect,
            position,
            cam))
        {
            introObjects[_currentIndex].SetActive(false);

            _currentIndex++;

            ShowNextHint();
        }
    }

    private void FinishIntro()
    {
        _isAnimating = false;

        _currentRect = null;
        _currentCanvas = null;

        if (_blockerObject != null)
            Destroy(_blockerObject);

        enabled = false;

        Debug.Log("Profile intro завершён!");
    }

    private void HideAllHints()
    {
        if (introObjects == null)
            return;

        foreach (GameObject obj in introObjects)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }

    private void OnDisable()
    {
        _isAnimating = false;

        if (_blockerObject != null)
            Destroy(_blockerObject);
    }
}
