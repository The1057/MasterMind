using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SubtaskCompleter : MonoBehaviour
{
    [SerializeField] public int taskIndex;     // номер задачи в списке tasks (с 0)
    public int subtaskIndex;  // номер подзадачи внутри неё (с 0)

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Complete);
    }

    public void Complete()
    {
        TaskManager23.CompleteSubtaskGlobal(taskIndex, subtaskIndex);
    }
}