using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TaskButtonUI : MonoBehaviour
{
    public Text taskTitleText;
    public GameObject dropdownContent; // контейнер с подзадачами
    public GameObject subTaskButtonPrefab;
    public Button toggleButton;

    private MainTask _task;
    private TaskManager _manager;
    private bool _isExpanded = false;

    public void Initialize(MainTask task, TaskManager manager)
    {
        _task = task;
        _manager = manager;
        taskTitleText.text = task.displayName;
        toggleButton.onClick.AddListener(ToggleDropdown);
        RefreshSubTasks();
    }

    private void ToggleDropdown()
    {
        _isExpanded = !_isExpanded;
        dropdownContent.SetActive(_isExpanded);
    }

    private void RefreshSubTasks()
    {
        // Очистить старые подзадачи
        foreach (Transform child in dropdownContent.transform)
            Destroy(child.gameObject);

        // Создать новые
        foreach (var sub in _task.subTasks)
        {
            GameObject subBtnObj = Instantiate(subTaskButtonPrefab, dropdownContent.transform);
            SubTaskButtonUI subBtn = subBtnObj.GetComponent<SubTaskButtonUI>();
            if (subBtn != null)
                subBtn.Initialize(sub, _manager);
        }
    }
}