
using UnityEngine;
using UnityEngine.UI;

public class ShowHintIfTaskDone : MonoBehaviour
{
    [Header("Объект, за которым следим")]
    [SerializeField] private GameObject targetImage;

    [Header("Последовательность подсказок")]
    [SerializeField] private GameObject[] hintObjects;

    [Header("Настройки сохранения")]
    [SerializeField] private string saveKey = "ShowHintIfTaskDone_Completed";

    private bool hasAlreadyRun;
    private int currentHintIndex = -1;

    private RectTransform currentHintRect;
    private Canvas currentHintCanvas;

    private void Start()
    {
        if (targetImage == null ||
            hintObjects == null ||
            hintObjects.Length == 0)
        {
            Debug.LogError("ShowHintIfTaskDone: Не назначены объекты!");
            enabled = false;
            return;
        }

        // Проверяем, запускались ли подсказки ранее
        hasAlreadyRun = PlayerPrefs.GetInt(saveKey, 0) == 1;

        // Скрываем все подсказки
        foreach (GameObject hint in hintObjects)
        {
            if (hint != null)
                hint.SetActive(false);
        }

        // Если уже запускались — больше не запускаем
        if (hasAlreadyRun)
            return;

        // Проверяем выполнение задания
        if (!targetImage.activeInHierarchy)
        {
            hasAlreadyRun = true;

            // Сохраняем блокировку
            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.Save();

            ShowHint(0);
        }
    }

    private void Update()
    {
        if (currentHintIndex < 0)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            CheckClick(Input.mousePosition);
        }
        else if (Input.touchCount > 0 &&
                 Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckClick(Input.GetTouch(0).position);
        }
    }

    private void ShowHint(int index)
    {
        if (index >= hintObjects.Length)
        {
            currentHintIndex = -1;
            currentHintRect = null;
            currentHintCanvas = null;
            return;
        }

        if (hintObjects[index] == null)
        {
            ShowHint(index + 1);
            return;
        }

        currentHintIndex = index;

        GameObject hint = hintObjects[index];
        hint.SetActive(true);

        currentHintRect = hint.GetComponent<RectTransform>();
        currentHintCanvas = hint.GetComponentInParent<Canvas>();
    }

    private void CheckClick(Vector2 position)
    {
        if (currentHintRect == null)
            return;

        Camera cam = null;

        if (currentHintCanvas != null &&
            currentHintCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = currentHintCanvas.worldCamera;
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(
            currentHintRect, position, cam))
        {
            hintObjects[currentHintIndex].SetActive(false);

            ShowHint(currentHintIndex + 1);
        }
    }
}
