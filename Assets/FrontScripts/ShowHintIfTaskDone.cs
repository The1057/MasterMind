
using UnityEngine;
using UnityEngine.UI;

public class ShowHintIfTaskDone : MonoBehaviour
{
    [Header("Объект, за которым следим")]
    [SerializeField] private GameObject targetImage;

    [Header("Последовательность подсказок")]
    [SerializeField] private GameObject[] hintObjects;

    private bool hintShown;
    private int currentHintIndex = -1;

    private RectTransform currentHintRect;
    private Canvas currentHintCanvas;

    private bool waitingForCanvas;

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

        // Скрываем все подсказки
        foreach (GameObject hint in hintObjects)
        {
            if (hint != null)
                hint.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Проверяем условие при каждом возвращении на Canvas
        waitingForCanvas = true;
    }

    private void Update()
    {
        // Проверяем условие после активации Canvas
        if (waitingForCanvas && !hintShown)
        {
            waitingForCanvas = false;

            // Проверяем, отключён ли родитель targetImage
            if (!targetImage.activeInHierarchy)
            {
                hintShown = true;
                ShowHint(0);
            }
        }

        // Проверяем нажатия на подсказки
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
