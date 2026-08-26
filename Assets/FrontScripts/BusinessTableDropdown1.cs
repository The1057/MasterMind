using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager23 : MonoBehaviour, ISaveLoadable
{
    [Header("Тестирование")]
    public bool debugLogs = true;

    [Header("Настройки префаба")]
    public GameObject subtaskPrefab;
    public string checkmarkObjectName = "Checkmark";

    [Header("Настройки")]
    public float expandDuration = 0.3f;
    public float childHeight = 50f;
    public float spacing = 4f;
    public float paddingTop = 8f;
    public float paddingBottom = 8f;

    [Header("Спрайты")]
    public Sprite completedSprite;
    public Sprite uncompletedSprite;
    public Sprite lockedOverlaySprite;
    public Sprite collapsedTaskSprite;    // Спрайт для свернутой задачи
    public Sprite expandedTaskSprite;     // Спрайт для раскрытой задачи

    [Header("Прогресс-бар (заполняющий элемент)")]
    public Image progressFillImage; // Дочерний Image, который будет "заполняться"

    [Header("Список задач")]
    public List<TaskData> tasks = new List<TaskData>();

    private int currentTaskIndex = 0;
    private int expandedTaskIndex = -1;

    [System.Serializable]
    public class SubtaskData
    {
        public string description;
        public Button navigationButton;
        public List<Button> completionButtons;
        [HideInInspector] public bool isCompleted = false;
        [HideInInspector] public Image autoCheckmark;
        [HideInInspector] public GameObject instantiatedUI;
    }

    [System.Serializable]
    public class TaskData
    {
        
        public Button headerButton;
        public List<SubtaskData> subtasks;
        public Image lockedOverlayImage;
        [HideInInspector] public Image headerButtonImage;
        [HideInInspector] public bool isExpanded = false;
        [HideInInspector] public RectTransform headerRect;
        [HideInInspector] public float originalHeight;
    }

    void Start()
    {
        InitializeTasks();
    }

    void InitializeTasks()
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            TaskData task = tasks[i];
            if (task.headerButton == null) continue;

            task.headerRect = task.headerButton.GetComponent<RectTransform>();
            task.originalHeight = task.headerRect.sizeDelta.y;
            task.headerButtonImage = task.headerButton.GetComponent<Image>();

            int taskIndex = i;
            task.headerButton.onClick.AddListener(() => ToggleExpand(taskIndex));

            foreach (var subtask in task.subtasks)
            {
                if (subtaskPrefab != null)
                {
                    subtask.instantiatedUI = Instantiate(subtaskPrefab, task.headerRect);
                    subtask.instantiatedUI.SetActive(false);

                    // 1. АВТО-ПОИСК ГАЛОЧКИ: ищем объект по имени внутри созданного префаба
                    Transform checkTransform = subtask.instantiatedUI.transform.Find(checkmarkObjectName);
                    if (checkTransform != null)
                        subtask.autoCheckmark = checkTransform.GetComponent<Image>();

                    // 2. Установка текста
                    var txt = subtask.instantiatedUI.GetComponentInChildren<TMP_Text>();
                    if (txt != null) txt.text = subtask.description;

                    // 3. Установка начального спрайта (важно!)
                    if (subtask.autoCheckmark != null)
                        subtask.autoCheckmark.sprite = subtask.isCompleted ? completedSprite : uncompletedSprite;
                }

                // Кнопка навигации (растягиваем как раньше)
                if (subtask.navigationButton != null && subtask.instantiatedUI != null)
                {
                    subtask.navigationButton.transform.SetParent(subtask.instantiatedUI.transform, false);
                    RectTransform navRt = subtask.navigationButton.GetComponent<RectTransform>();
                    navRt.anchorMin = Vector2.zero; navRt.anchorMax = Vector2.one;
                    navRt.offsetMin = Vector2.zero; navRt.offsetMax = Vector2.zero;
                }

                // Подписка кнопок выполнения
                foreach (var compButton in subtask.completionButtons)
                {
                    if (compButton != null)
                        compButton.onClick.AddListener(() => CompleteSubtask(subtask));
                }
            }

            if (task.lockedOverlayImage != null)
            {
                task.lockedOverlayImage.sprite = lockedOverlaySprite;
                task.lockedOverlayImage.gameObject.SetActive(true);
            }
        }
        UpdateTaskState(0);
        UpdateProgressFill();
    }

    void ToggleExpand(int taskIndex)
    {
        if (taskIndex > currentTaskIndex) return;
        StopAllCoroutines();
        if (expandedTaskIndex == taskIndex) { StartCoroutine(Contract(tasks[taskIndex])); expandedTaskIndex = -1; }
        else
        {
            if (expandedTaskIndex != -1) StartCoroutine(Contract(tasks[expandedTaskIndex], () => StartCoroutine(Expand(tasks[taskIndex], taskIndex))));
            else StartCoroutine(Expand(tasks[taskIndex], taskIndex));
        }
    }

    public void CompleteSubtask(SubtaskData subtask)
    {
        if (subtask.isCompleted) return;

        subtask.isCompleted = true;
        if (subtask.autoCheckmark != null)
            subtask.autoCheckmark.sprite = completedSprite;

        TaskData parentTask = tasks.FirstOrDefault(t => t.subtasks.Contains(subtask));
        if (parentTask != null)
        {
            CheckTaskCompletion(parentTask);
            UpdateProgressFill();
        }
    }

    private TaskData FindParentTask(SubtaskData subtask)
    {
        foreach (var task in tasks)
        {
            if (task.subtasks.Contains(subtask))
            {
                return task;
            }
        }
        return null;
    }

    void CheckTaskCompletion(TaskData task)
    {
        if (task.subtasks.All(st => st.isCompleted))
        {
            int idx = tasks.IndexOf(task);
            if (idx + 1 < tasks.Count)
            {
                currentTaskIndex = idx + 1;
                UpdateTaskState(currentTaskIndex);
            }
        }
    }

    void UpdateTaskState(int taskIndex)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].lockedOverlayImage != null)
                tasks[i].lockedOverlayImage.gameObject.SetActive(i > taskIndex);
        }
    }

    // Новые методы для обновления прогресса
    void UpdateProgressFill()
    {
        if (progressFillImage == null) return;

        TaskData currentTask = tasks[currentTaskIndex];
        int total = currentTask.subtasks.Count;
        if (total == 0)
        {
            progressFillImage.fillAmount = 0f;
            return;
        }

        int completed = currentTask.subtasks.Count(st => st.isCompleted);
        float progress = (float)completed / total;

        // Если Image использует Type = Filled (Image Type = Filled)
        progressFillImage.fillAmount = progress;
    }

    void ResetProgressFill()
    {
        if (progressFillImage != null)
        {
            progressFillImage.fillAmount = 0f;
        }
    }

    public void save(ref saveData saveData)
    {
        saveData.TasksData.Clear();

        for (int i = 0; i < tasks.Count; i++)
        {
            TaskData task = tasks[i];
            TaskSaveData taskSave = new TaskSaveData();
            taskSave.taskIndex = i;
            taskSave.isExpanded = task.isExpanded;

            bool allSubtasksCompleted = true;
            for (int j = 0; j < task.subtasks.Count; j++)
            {
                SubtaskData subtask = task.subtasks[j];
                SubtaskSaveData subtaskSave = new SubtaskSaveData();
                subtaskSave.subtaskIndex = j;
                subtaskSave.isCompleted = subtask.isCompleted;
                taskSave.subtasksData.Add(subtaskSave);

                if (!subtask.isCompleted)
                {
                    allSubtasksCompleted = false;
                }
            }
            taskSave.isCompleted = allSubtasksCompleted;
            saveData.TasksData.Add(taskSave);
        }
    }

    public void load(saveData loadData)
    {
        if (loadData.TasksData.Count == 0) return;

        foreach (var taskSave in loadData.TasksData)
        {
            if (taskSave.taskIndex < tasks.Count)
            {
                TaskData task = tasks[taskSave.taskIndex];

                foreach (var subtaskSave in taskSave.subtasksData)
                {
                    if (subtaskSave.subtaskIndex < task.subtasks.Count)
                    {
                        task.subtasks[subtaskSave.subtaskIndex].isCompleted = subtaskSave.isCompleted;
                    }
                }

                CheckTaskCompletion(task);
            }
        }

        for (int i = 0; i < tasks.Count; i++)
        {
            bool allSubtasksCompleted = tasks[i].subtasks.All(s => s.isCompleted);
            if (!allSubtasksCompleted)
            {
                currentTaskIndex = i;
                break;
            }
            if (i == tasks.Count - 1 && allSubtasksCompleted)
            {
                currentTaskIndex = tasks.Count - 1;
            }
        }

        UpdateTaskState(currentTaskIndex);
        UpdateProgressFill(); // После загрузки обновляем прогресс
    }



    IEnumerator Expand(TaskData task, int taskIndex)
    {
        task.isExpanded = true;
        expandedTaskIndex = taskIndex;
        if (task.headerButtonImage != null && expandedTaskSprite != null) task.headerButtonImage.sprite = expandedTaskSprite;

        float targetHeight = task.originalHeight + paddingTop + paddingBottom + (task.subtasks.Count * childHeight) + ((task.subtasks.Count - 1) * spacing);
        yield return StartCoroutine(AnimateHeight(task.headerRect, task.originalHeight, targetHeight));

        float yPos = -task.originalHeight - paddingTop;
        foreach (var subtask in task.subtasks)
        {
            if (subtask.instantiatedUI == null) continue;

            subtask.instantiatedUI.SetActive(true);
            RectTransform rt = subtask.instantiatedUI.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f); rt.anchorMax = new Vector2(1f, 1f); rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(0f, childHeight);
            rt.anchoredPosition = new Vector2(0, yPos);

            yPos -= (childHeight + spacing);

            if (subtask.autoCheckmark != null)
                subtask.autoCheckmark.sprite = subtask.isCompleted ? completedSprite : uncompletedSprite;
        }
    }

    IEnumerator Contract(TaskData task, System.Action onComplete = null)
    {
        task.isExpanded = false;
        if (task.headerButtonImage != null && collapsedTaskSprite != null) task.headerButtonImage.sprite = collapsedTaskSprite;
        foreach (var subtask in task.subtasks) if (subtask.instantiatedUI != null) subtask.instantiatedUI.SetActive(false);
        yield return StartCoroutine(AnimateHeight(task.headerRect, task.headerRect.sizeDelta.y, task.originalHeight));
        onComplete?.Invoke();
    }

    IEnumerator AnimateHeight(RectTransform rt, float start, float end)
    {
        float elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, Mathf.Lerp(start, end, Mathf.SmoothStep(0f, 1f, elapsed / expandDuration)));
            yield return null;
        }
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, end);
    }
    // Метод для пропуска следующей невыполненной подзадачи
    public void DebugSkipNextSubtask()
    {
        if (currentTaskIndex >= tasks.Count)
        {
            if (debugLogs) Debug.Log("Все задачи уже выполнены!");
            return;
        }

        TaskData currentTask = tasks[currentTaskIndex];
        // Находим первую подзадачу, которая еще не завершена
        SubtaskData nextSubtask = currentTask.subtasks.FirstOrDefault(s => !s.isCompleted);

        if (nextSubtask != null)
        {
            if (debugLogs) Debug.Log($"Тест: Пропускаем подзадачу '{nextSubtask.description}'");
            CompleteSubtask(nextSubtask);
        }
        else
        {
            // Если в текущей задаче всё выполнено (на всякий случай)
            if (debugLogs) Debug.Log("В текущей задаче нет невыполненных подзадач.");
        }
    }

    // Метод для полного сброса прогресса
    public void DebugResetAllProgress()
    {
        saveData currentFileState = saveLoadManager.instance.loadData();

        if (currentFileState != null)
        {
            // 2. Очищаем конкретно список данных задач
            if (currentFileState.TasksData != null)
            {
                currentFileState.TasksData.Clear();
                Debug.Log("<color=yellow>Данные задач в файле очищены.</color>");
            }

            saveLoadManager.instance.saveData(currentFileState);
        }

        // 4. Теперь сбрасываем визуальное состояние в текущей сцене
        currentTaskIndex = 0;
        expandedTaskIndex = -1;

        foreach (var task in tasks)
        {
            task.isExpanded = false;
            // Возвращаем высоту заголовков
            if (task.headerRect != null)
                task.headerRect.sizeDelta = new Vector2(task.headerRect.sizeDelta.x, task.originalHeight);

            if (task.headerButtonImage != null && collapsedTaskSprite != null)
                task.headerButtonImage.sprite = collapsedTaskSprite;

            foreach (var subtask in task.subtasks)
            {
                subtask.isCompleted = false;
                // Сбрасываем галочки на пустые
                if (subtask.autoCheckmark != null && uncompletedSprite != null)
                    subtask.autoCheckmark.sprite = uncompletedSprite;

                // Прячем подзадачи
                if (subtask.instantiatedUI != null)
                    subtask.instantiatedUI.SetActive(false);
            }
        }

        // Обнуляем прогресс-бар и закрытые замочки
        UpdateTaskState(0);
        UpdateProgressFill();

        Debug.Log("<color=green>Прогресс задач сброшен локально и в файле!</color>");
    }
}