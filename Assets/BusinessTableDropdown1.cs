using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TaskManager23 : MonoBehaviour, ISaveLoadable
{
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

    [Header("Прогресс-бар (заполняющий элемент)")]
    public Image progressFillImage; // Дочерний Image, который будет "заполняться"

    [Header("Список задач")]
    public List<TaskData> tasks = new List<TaskData>();

    private int currentTaskIndex = 0;
    private int expandedTaskIndex = -1;

    [System.Serializable]
    public class SubtaskData
    {
        public GameObject subtaskObject;
        public List<Button> completionButtons;
        public Image completionImage;
        [HideInInspector] public bool isCompleted = false;
    }

    [System.Serializable]
    public class TaskData
    {
        public Button headerButton;
        public List<SubtaskData> subtasks;
        public Image lockedOverlayImage;
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

            if (task.headerButton == null)
            {
                Debug.LogError($"Задача {i + 1}: Кнопка-заголовок не назначена!");
                continue;
            }

            task.headerRect = task.headerButton.GetComponent<RectTransform>();
            task.originalHeight = task.headerRect.sizeDelta.y;

            int index = i;
            task.headerButton.onClick.AddListener(() => ToggleExpand(index));

            foreach (var subtask in task.subtasks)
            {
                if (subtask.subtaskObject != null)
                {
                    subtask.subtaskObject.SetActive(false);
                }

                foreach (var button in subtask.completionButtons)
                {
                    button.onClick.AddListener(() => CompleteSubtask(subtask));
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
        if (taskIndex > currentTaskIndex)
        {
            return;
        }

        StopAllCoroutines();

        if (expandedTaskIndex == taskIndex)
        {
            StartCoroutine(Contract(tasks[taskIndex]));
            expandedTaskIndex = -1;
        }
        else
        {
            if (expandedTaskIndex != -1)
            {
                StartCoroutine(Contract(tasks[expandedTaskIndex], () => StartCoroutine(Expand(tasks[taskIndex], taskIndex))));
            }
            else
            {
                StartCoroutine(Expand(tasks[taskIndex], taskIndex));
            }
        }
    }

    void CompleteSubtask(SubtaskData subtaskToComplete)
    {
        if (subtaskToComplete.isCompleted)
        {
            return;
        }

        subtaskToComplete.isCompleted = true;

        if (subtaskToComplete.completionImage != null && completedSprite != null)
        {
            subtaskToComplete.completionImage.sprite = completedSprite;
        }

        TaskData parentTask = FindParentTask(subtaskToComplete);
        if (parentTask != null)
        {
            CheckTaskCompletion(parentTask);
            UpdateProgressFill(); // Обновляем прогресс
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
        bool allSubtasksCompleted = true;
        foreach (var subtask in task.subtasks)
        {
            if (!subtask.isCompleted)
            {
                allSubtasksCompleted = false;
                break;
            }
        }

        if (allSubtasksCompleted)
        {
            int completedTaskIndex = tasks.IndexOf(task);
            if (completedTaskIndex != -1)
            {
                Debug.Log($"Задача '{task.headerButton.name}' выполнена!");

                if (completedTaskIndex + 1 < tasks.Count)
                {
                    currentTaskIndex = completedTaskIndex + 1;
                    UpdateTaskState(currentTaskIndex);
                    ResetProgressFill(); // Сброс при переходе к следующей задаче
                }
            }
        }
    }

    void UpdateTaskState(int taskIndex)
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            if (i < taskIndex)
            {
                if (tasks[i].lockedOverlayImage != null)
                    tasks[i].lockedOverlayImage.gameObject.SetActive(false);
            }
            else if (i == taskIndex)
            {
                if (tasks[i].lockedOverlayImage != null)
                    tasks[i].lockedOverlayImage.gameObject.SetActive(false);
            }
            else
            {
                if (tasks[i].lockedOverlayImage != null)
                    tasks[i].lockedOverlayImage.gameObject.SetActive(true);
            }
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

    // Реализация интерфейса сохранения/загрузки (оставлено без изменений)
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
                        UpdateSubtaskSprite(task.subtasks[subtaskSave.subtaskIndex]);
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

    private void UpdateSubtaskSprite(SubtaskData subtask)
    {
        if (subtask.completionImage != null)
        {
            subtask.completionImage.sprite = subtask.isCompleted ? completedSprite : uncompletedSprite;
        }
    }

    IEnumerator Expand(TaskData task, int taskIndex)
    {
        task.isExpanded = true;
        expandedTaskIndex = taskIndex;

        float contentHeight = paddingTop + paddingBottom;
        if (task.subtasks.Count > 0)
        {
            contentHeight += task.subtasks.Count * childHeight;
            contentHeight += (task.subtasks.Count - 1) * spacing;
        }
        float targetHeight = task.originalHeight + contentHeight;

        float elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / expandDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            task.headerRect.sizeDelta = new Vector2(task.headerRect.sizeDelta.x, Mathf.Lerp(task.originalHeight, targetHeight, smoothT));
            yield return null;
        }
        task.headerRect.sizeDelta = new Vector2(task.headerRect.sizeDelta.x, targetHeight);

        float yPos = -task.originalHeight - paddingTop;
        foreach (var subtask in task.subtasks)
        {
            if (subtask.subtaskObject == null) continue;

            subtask.subtaskObject.SetActive(true);
            RectTransform rt = subtask.subtaskObject.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.SetParent(task.headerRect, false);
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(1f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.sizeDelta = new Vector2(0f, childHeight);
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, yPos);
            }
            yPos -= (childHeight + spacing);

            if (subtask.isCompleted)
            {
                if (subtask.completionImage != null && completedSprite != null)
                {
                    subtask.completionImage.sprite = completedSprite;
                }
            }
            else
            {
                if (subtask.completionImage != null && uncompletedSprite != null)
                {
                    subtask.completionImage.sprite = uncompletedSprite;
                }
            }
        }
    }

    IEnumerator Contract(TaskData task, System.Action onComplete = null)
    {
        task.isExpanded = false;

        foreach (var subtask in task.subtasks)
        {
            if (subtask.subtaskObject != null)
            {
                subtask.subtaskObject.SetActive(false);
            }
        }

        float startHeight = task.headerRect.sizeDelta.y;
        float targetHeight = task.originalHeight;
        float elapsed = 0f;
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / expandDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);
            task.headerRect.sizeDelta = new Vector2(task.headerRect.sizeDelta.x, Mathf.Lerp(startHeight, targetHeight, smoothT));
            yield return null;
        }
        task.headerRect.sizeDelta = new Vector2(task.headerRect.sizeDelta.x, targetHeight);

        onComplete?.Invoke();
    }
}