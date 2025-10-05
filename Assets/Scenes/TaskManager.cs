using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SubTask
{
    public string id; // уникальный ID, например "task1_sub1"
    public string displayName;
    public bool isCompleted = false;
    public string targetCanvasName; // или ссылка на Canvas, или SceneName
}

[System.Serializable]
public class MainTask
{
    public string id; // "task1", "task2"
    public string displayName;
    public List<SubTask> subTasks = new List<SubTask>();
    public bool isUnlocked = false;

    public bool IsCompleted()
    {
        return subTasks.Count > 0 && subTasks.All(st => st.isCompleted);
    }
}
public class TaskManager : MonoBehaviour
{
    public List<MainTask> tasks = new List<MainTask>();
    public GameObject taskButtonPrefab; // префаб кнопки "Задача N"
    public Transform taskButtonsParent; // где будут кнопки задач

    private void Start()
    {
        LoadProgress(); // можно из PlayerPrefs или SaveSystem
        UnlockFirstTask();
        RefreshUI();
    }

    public void CompleteSubTask(string subTaskId)
    {
        foreach (var task in tasks)
        {
            var sub = task.subTasks.Find(s => s.id == subTaskId);
            if (sub != null)
            {
                sub.isCompleted = true;
                SaveProgress();
                CheckUnlockNextTask(task);
                RefreshUI();
                break;
            }
        }
    }

    private void CheckUnlockNextTask(MainTask completedTask)
    {
        int index = tasks.IndexOf(completedTask);
        if (index >= 0 && index + 1 < tasks.Count && completedTask.IsCompleted())
        {
            tasks[index + 1].isUnlocked = true;
            SaveProgress();
        }
    }

    private void UnlockFirstTask()
    {
        if (tasks.Count > 0)
            tasks[0].isUnlocked = true;
    }

    public void RefreshUI()
    {
        // Очистить старые кнопки
        foreach (Transform child in taskButtonsParent)
            Destroy(child.gameObject);

        // Создать кнопки задач
        for (int i = 0; i < tasks.Count; i++)
        {
            if (tasks[i].isUnlocked)
            {
                GameObject btnObj = Instantiate(taskButtonPrefab, taskButtonsParent);
                TaskButtonUI buttonUI = btnObj.GetComponent<TaskButtonUI>();
                if (buttonUI != null)
                    buttonUI.Initialize(tasks[i], this);
            }
        }
    }

    // Простой способ сохранения (можно заменить на JSON/PlayerPrefs/SaveSystem)
    private void SaveProgress()
    {
        // Например, через PlayerPrefs или сериализацию в JSON
        // Для простоты сейчас пропустим, но ниже покажу пример
    }

    private void LoadProgress()
    {
        // Загрузка прогресса
    }
}