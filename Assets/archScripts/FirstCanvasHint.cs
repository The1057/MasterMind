
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FirstCanvasHint : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private GameObject hintObject;

    private const string ShownKey = "FirstCanvasHint_Shown";

    private void Start()
    {
        // Проверяем, показывалась ли подсказка ранее
        bool alreadyShown = PlayerPrefs.GetInt(ShownKey, 0) == 1;

        // Показываем только при первом открытии
        if (hintObject != null)
        {
            hintObject.SetActive(!alreadyShown);
        }
    }

    private void Update()
    {
        if (hintObject == null || !hintObject.activeSelf)
            return;

        // Проверяем нажатие мышью или тап на Android
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current == null)
                return;

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                if (result.gameObject == hintObject ||
                    result.gameObject.transform.IsChildOf(hintObject.transform))
                {
                    CloseHint();
                    break;
                }
            }
        }
    }

    private void CloseHint()
    {
        hintObject.SetActive(false);

        // Запоминаем, что подсказка уже была показана
        PlayerPrefs.SetInt(ShownKey, 1);
        PlayerPrefs.Save();
    }
}
