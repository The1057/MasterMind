using UnityEngine;
using UnityEngine.UI;

public class SubTaskButtonUI : MonoBehaviour
{
    public Text subTaskText;
    public Image checkmark; // например, галочка, если выполнено
    public Button button;

    private SubTask _subTask;
    private TaskManager _manager;

    public void Initialize(SubTask subTask, TaskManager manager)
    {
        _subTask = subTask;
        _manager = manager;

        string status = _subTask.isCompleted ? " у" : "";
        subTaskText.text = _subTask.displayName + status;
        checkmark.gameObject.SetActive(_subTask.isCompleted);
        button.interactable = !_subTask.isCompleted; // нельзя нажать дважды

        button.onClick.AddListener(OnSubTaskClicked);
    }

    private void OnSubTaskClicked()
    {
        _manager.CompleteSubTask(_subTask.id);
    }
}