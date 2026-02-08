using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// Наследуемся от ISaveLoadable, чтобы saveLoadManager видел этот скрипт
public class TutorManager : MonoBehaviour, ISaveLoadable
{
    public static TutorManager instance;
    public enum AnchorType { Center, TopLeft, TopRight, BottomLeft, BottomRight, Top, Bottom, Left, Right }

    [System.Serializable]
    public struct HintStep
    {
        public string text;
        public bool useArrow;
        public AnchorType arrowAnchor;
        public Vector2 arrowAnchoredPosition; // Позиция внутри Канваса
        public float arrowRotation;
    }

    [System.Serializable]
    public struct HintSeries
    {
        public int seriesID; // Уникальный ID для сохранения
        public string seriesName;
        public List<HintStep> steps;
    }

    [Header("UI & Hierarchy")]
    public Transform hintCanvasTransform; // Сюда перетащи Canvas или панель, где должна быть стрелка
    public GameObject hintPanel;
    public TextMeshProUGUI hintText;
    public GameObject arrowPrefab;

    [Header("Data")]
    public List<HintSeries> allSeries;

    private List<int> _completedSeries = new List<int>();
    private GameObject _currentArrow;
    private RectTransform _arrowRT;
    private int _currentSeriesIndex = -1;
    private int _currentStepIndex = -1;
    private bool _isHintActive = false;

    private void Awake()
    {
        // Логика Синглтона: если менеджер уже есть, удаляем дубликат
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Объект не удалится при смене сцены
            SceneManager.sceneLoaded += OnSceneLoaded; // Подписываемся на событие загрузки сцены
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        hintPanel.SetActive(false);
        if (arrowPrefab != null)
        {
            _currentArrow = Instantiate(arrowPrefab, hintCanvasTransform);
            _arrowRT = _currentArrow.GetComponent<RectTransform>();
            _currentArrow.SetActive(false);
        }
        Invoke("CheckFirstStart", 0.1f);
    }

    private void CheckFirstStart()
    {
        // Если ID 0 еще нет в списке завершенных — запускаем
        if (!_completedSeries.Contains(0))
        {
            StartSeries(0);
        }
    }

    void Update()
    {
        if (_isHintActive && Input.GetMouseButtonDown(0))
        {
            NextHint();
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Очищаем старую стрелку, если она вдруг осталась (хотя Unity ее удалит сама)
        _currentArrow = null;

        // Ищем Canvas. Если он может быть выключен, Find его не найдет!
        // Лучше использовать тег или убедиться, что объект включен.
        GameObject canvasObj = GameObject.Find("ForTutor");

        if (canvasObj != null)
        {
            hintCanvasTransform = canvasObj.transform;

            // Если обучение активно в момент перехода — переподключаем UI
            if (_isHintActive)
            {
                ReinitUIOnNewScene(canvasObj);
            }
        }
        else
        {
            Debug.LogWarning("TutorManager: На новой сцене не найден объект ForTutor!");
        }
    }

    private void ReinitUIOnNewScene(GameObject newCanvas)
    {
        // Ищем панель. Убедись, что имя "Трейд" совпадает на 100%
        Transform panelTransform = newCanvas.transform.Find("Трейд");

        if (panelTransform != null)
        {
            hintPanel = panelTransform.gameObject;
            hintText = hintPanel.GetComponentInChildren<TextMeshProUGUI>();

            // Включаем панель, если мы посреди обучения
            hintPanel.SetActive(!string.IsNullOrEmpty(allSeries[_currentSeriesIndex].steps[_currentStepIndex].text));
        }

        // Создаем НОВУЮ стрелку для НОВОЙ сцены
        if (arrowPrefab != null)
        {
            _currentArrow = Instantiate(arrowPrefab, hintCanvasTransform);
            _arrowRT = _currentArrow.GetComponent<RectTransform>();
            _currentArrow.SetActive(false);

            // Сразу обновляем позицию стрелки под новую сцену
            UpdateHintUI();
        }
    }

    // Тот самый метод для кнопок (вызывай его в OnClick)
    public void StartSeries(int seriesId)
    {
        // Если уже проходили эту серию — ничего не делаем
        if (_completedSeries.Contains(seriesId)) return;

        int index = allSeries.FindIndex(s => s.seriesID == seriesId);
        if (index == -1) return;

        _currentSeriesIndex = index;
        _currentStepIndex = 0;
        _isHintActive = true;
        hintPanel.SetActive(true);

        UpdateHintUI();
    }

    public void StartTutorialChain(string sequence)
    {
        // sequence - это строка с ID через запятую, например "1,2,5"
        string[] ids = sequence.Split(',');
        foreach (string idStr in ids)
        {
            int id = int.Parse(idStr);
            if (!_completedSeries.Contains(id))
            {
                StartSeries(id);
                return; // Запускаем первую не пройденную и выходим
            }
        }
    }
    private void NextHint()
    {
        _currentStepIndex++;
        if (_currentStepIndex < allSeries[_currentSeriesIndex].steps.Count)
        {
            UpdateHintUI();
        }
        else
        {
            FinishSeries();
        }
    }

    private void UpdateHintUI()
    {
        HintStep currentStep = allSeries[_currentSeriesIndex].steps[_currentStepIndex];
        
        bool hasText = !string.IsNullOrEmpty(currentStep.text);
        if (hintPanel != null)
        {
            hintPanel.SetActive(hasText);
            if (hasText) hintText.text = currentStep.text;
        }

        if (_currentArrow != null && currentStep.useArrow)
        {
            _currentArrow.SetActive(true);
            SetAnchor(_arrowRT, currentStep.arrowAnchor); // Устанавливаем якоря
            
            _arrowRT.anchoredPosition = currentStep.arrowAnchoredPosition;
            _arrowRT.localRotation = Quaternion.Euler(0, 0, currentStep.arrowRotation);
            
        }
        else if (_currentArrow != null)
        {
            _currentArrow.SetActive(false);
        }
    }

    // Вспомогательный метод для программной смены якорей
    private void SetAnchor(RectTransform rt, AnchorType type)
    {
        switch (type)
        {
            case AnchorType.Center:
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
                break;
            case AnchorType.TopLeft:
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 1);
                break;
            case AnchorType.TopRight:
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1, 1);
                break;
            case AnchorType.BottomLeft:
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0, 0);
                break;
            case AnchorType.BottomRight:
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(1, 0);
                break;
            case AnchorType.Top:
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 1);
                break;
            case AnchorType.Bottom:
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0);
                break;
        }
    }

    private void FinishSeries()
    {
        // Добавляем ID в список пройденных
        int finishedID = allSeries[_currentSeriesIndex].seriesID;
        if (!_completedSeries.Contains(finishedID))
        {
            _completedSeries.Add(finishedID);
        }

        _isHintActive = false;
        hintPanel.SetActive(false);
        if (_currentArrow != null) _currentArrow.SetActive(false);

        // Принудительно сохраняем игру после прохождения серии
        if (saveLoadManager.instance != null) saveLoadManager.instance.saveGame();
    }

    // --- Реализация интерфейса ISaveLoadable ---

    public void load(saveData data)
    {
        if (data.completedTutorialSeries != null)
        {
            _completedSeries = new List<int>(data.completedTutorialSeries);
        }
    }

    public void save(ref saveData data)
    {
        data.completedTutorialSeries = new List<int>(_completedSeries);
    }
}