using UnityEngine;
using TMPro;
using System.Collections; // обязательно
using UnityEngine.EventSystems;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [Header("UI Elements")]
    public GameObject infoPanel;
    public TextMeshProUGUI buildingNameText;
    public TextMeshProUGUI descriptionText;

    private buildingInfo currentBuilding;

    // Это поле — чтобы не запускать анимацию повторно на одном здании
    private Transform currentlyAnimatingBuilding = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (infoPanel != null)
        {
            infoPanel.SetActive(false);

            // Добавляем клик по панели для закрытия
            EventTrigger trigger = infoPanel.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = infoPanel.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { ClosePanel(); });
            trigger.triggers.Add(entry);
        }
    }

    public void OnBuildingClicked(buildingInfo data, Transform buildingTransform)
    {
        currentBuilding = data;

        // Показываем панель
        if (infoPanel != null && buildingNameText != null && descriptionText != null)
        {
            buildingNameText.text = data.buildingName;
            descriptionText.text = data.description;
            infoPanel.SetActive(true);
        }

        Debug.Log($"Clicked: {data.buildingName}\n{data.description}");

        // Запускаем анимацию ТОЛЬКО если это здание не анимируется прямо сейчас
        if (buildingTransform != null && currentlyAnimatingBuilding != buildingTransform)
        {
            currentlyAnimatingBuilding = buildingTransform;
            StartCoroutine(ScalePulseRoutine(buildingTransform));
        }
    }

    public void ClosePanel()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }

    // Анимация масштабирования — не даёт запускаться повторно
    private IEnumerator ScalePulseRoutine(Transform target)
    {
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * 1.03f; 

        float duration = 0.1f;
        float elapsed = 0f;

        // Фаза 1: увеличение
        while (elapsed < duration)
        {
            target.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localScale = targetScale;

        elapsed = 0f;

        // Фаза 2: возврат к исходному размеру
        while (elapsed < duration)
        {
            target.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        target.localScale = originalScale;

        // Сбрасываем ссылку — теперь можно снова анимировать это здание
        if (currentlyAnimatingBuilding == target)
            currentlyAnimatingBuilding = null;
    }
}