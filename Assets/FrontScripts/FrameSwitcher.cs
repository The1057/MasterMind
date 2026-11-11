
using UnityEngine;

public class FrameSwitcher : MonoBehaviour
{
    [Header("Фрейм, который нужно открыть")]
    [SerializeField] private GameObject targetFrame;

    public void SwitchFrame()
    {
        if (targetFrame == null)
        {
            Debug.LogError("FrameSwitcher: Не назначен Target Frame!");
            return;
        }

        // Находим все фреймы внутри родительского объекта
        Transform parent = targetFrame.transform.parent;

        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                child.gameObject.SetActive(false);
            }
        }

        // Открываем нужный фрейм
        targetFrame.SetActive(true);
    }
}
