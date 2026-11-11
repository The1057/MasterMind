
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FirstCanvasHintMap : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private GameObject hintObject;

    [Header("Уникальный ID подсказки")]
    [SerializeField] private string hintID = "Canvas1";

    private string ShownKey => "FirstCanvasHint_" + hintID;

    private void Start()
    {
        bool alreadyShown = PlayerPrefs.GetInt(ShownKey, 0) == 1;

        if (hintObject != null)
            hintObject.SetActive(!alreadyShown);
    }

    private void Update()
    {
        if (hintObject == null || !hintObject.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current == null)
                return;

            PointerEventData pointerData =
                new PointerEventData(EventSystem.current);

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
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

        PlayerPrefs.SetInt(ShownKey, 1);
        PlayerPrefs.Save();
    }
}
