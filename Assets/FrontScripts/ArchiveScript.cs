using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArchiveManager : MonoBehaviour, ISaveLoadable
{
    [Header("Настройки префаба")]
    public GameObject platePrefab;
    public GameObject firstSpecialPlatePrefab;
    public string slot1Name = "Slot1";
    public string slot2Name = "Slot2";

    [Header("Глобальные кнопки управления этапами")]
    public Button globalNextButton; // Кнопка "Вперед"
    public Button globalPrevButton; // Кнопка "Назад"

    [Header("scr")]
    public ScrollRect scrollRect;

    [System.Serializable]
    public class PlateStatus
    {
        public string plateID;
        public string title;
        public Button leftNavButton;
        public Button rightNavButton;
        [HideInInspector] public bool leftDone;
        [HideInInspector] public bool rightDone;
        [HideInInspector] public ArchivePlateUI uiElement;
    }

    [System.Serializable]
    public class StageData
    {
        public string stageName;
        public GameObject stageCanvas;
        public Transform platesParent;
        public List<PlateStatus> plates;
    }

    public List<StageData> stages = new List<StageData>();
    private int activeStageIndex = 0;

    void Start()
    {
        InitializeArchive();
        // При старте показываем текущий этап
        ShowStage(activeStageIndex);
    }

    void InitializeArchive()
    {
        foreach (var stage in stages)
        {
            // Настройка кнопок
            if (globalNextButton != null)
            {
                globalNextButton.onClick.RemoveAllListeners();
                globalNextButton.onClick.AddListener(ShowNextStage);
            }
            if (globalPrevButton != null)
            {
                globalPrevButton.onClick.RemoveAllListeners();
                globalPrevButton.onClick.AddListener(ShowPrevStage);
            }

            foreach (var plate in stage.plates)
            {
                bool isFirstPlate = (stages.IndexOf(stage) == 0 && stage.plates.IndexOf(plate) == 0);

                // Выбираем префаб
                GameObject prefabToUse = isFirstPlate ? firstSpecialPlatePrefab : platePrefab;

                GameObject instance = Instantiate(prefabToUse, stage.platesParent);
                instance.transform.SetAsFirstSibling();

                // Находим скрипт управления UI на плашке
                plate.uiElement = instance.GetComponent<ArchivePlateUI>();

                // --- ИСПРАВЛЕННЫЙ ПОИСК ЗАГОЛОВКА ---
                // Ищем TMP_Text среди всех детей префаба, даже вложенных
                TMP_Text foundText = instance.GetComponentInChildren<TMP_Text>(true);

                if (foundText != null)
                {
                    foundText.text = plate.title;
                    // На всякий случай обновляем ссылку в ArchivePlateUI, если она там есть
                    if (plate.uiElement != null) plate.uiElement.titleText = foundText;
                }
                else
                {
                    Debug.LogWarning($"Архив: Не нашел текстовый компонент в префабе плашки '{plate.title}'!");
                }

                MoveButtonToSlot(instance, slot1Name, plate.leftNavButton);
                MoveButtonToSlot(instance, slot2Name, plate.rightNavButton);
            }
        }
        RefreshVisuals();
    }

    private void ShowNextStage()
    {
        if (activeStageIndex < stages.Count - 1)
        {
            activeStageIndex++;
            ShowStage(activeStageIndex);
            saveLoadManager.instance.saveGame(); // Сохраняем, на каком этапе остановились
        }
    }

    private void ShowPrevStage()
    {
        if (activeStageIndex > 0)
        {
            activeStageIndex--;
            ShowStage(activeStageIndex);
            saveLoadManager.instance.saveGame();
        }
    }

    private void ShowStage(int index)
    {
        for (int i = 0; i < stages.Count; i++)
        {
            bool isActive = (i == index);

            if (stages[i].stageCanvas != null)
                stages[i].stageCanvas.SetActive(isActive);

            // Если это активный этап, меняем content у общего ScrollRect
            if (isActive && scrollRect != null && stages[i].platesParent != null)
            {
                scrollRect.content = stages[i].platesParent as RectTransform;
                scrollRect.normalizedPosition = Vector2.zero;
            }
        }

        RefreshVisuals();
    }

    void MoveButtonToSlot(GameObject plate, string slotName, Button btn)
    {
        if (btn == null)
        {
            Debug.LogWarning($"Архив: Кнопка не назначена для одной из плашек!");
            return;
        }

        // Ищем слот по всей иерархии префаба, а не только сверху
        Transform slot = null;
        Transform[] allChildren = plate.GetComponentsInChildren<Transform>(true);
        foreach (var child in allChildren)
        {
            if (child.name == slotName)
            {
                slot = child;
                break;
            }
        }

        if (slot != null)
        {
            // ПЕРЕНОС КНОПКИ
            btn.transform.SetParent(slot, false);

            // Сбрасываем все параметры, чтобы она встала ровно
            btn.transform.SetAsLastSibling();
            RectTransform rt = btn.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one; // Важно сбросить масштаб на 1

            btn.gameObject.SetActive(true);

            //Debug.Log($"<color=green>Кнопка {btn.name} успешно перенесена в {slotName}</color>");
        }
        else
        {
            // Если ты видишь это сообщение в консоли - значит имя слота в префабе не совпадает
            Debug.LogError($"<color=red>Архив ОШИБКА: Не нашел объект с именем '{slotName}' внутри префаба плашки!</color>");
        }
    }

    // --- НОВЫЕ МЕТОДЫ ДЛЯ ЛЮБЫХ КНОПОК ---

    // В инспекторе любой кнопки: ArchiveManager -> MarkLeftID, в поле пишешь ID плашки
    public void MarkLeftID(string id) { SetSideByID(id, true); }
    public void MarkRightID(string id) { SetSideByID(id, false); }

    private void SetSideByID(string id, bool isLeft)
    {
        foreach (var stage in stages)
        {
            foreach (var plate in stage.plates)
            {
                if (plate.plateID == id)
                {
                    if (isLeft) plate.leftDone = true; else plate.rightDone = true;
                    RefreshVisuals();
                    saveLoadManager.instance.saveGame();
                    Debug.Log($"<color=green>Архив: {id} ({(isLeft ? "Лево" : "Право")}) пройдено!</color>");
                    return;
                }
            }
        }
        Debug.LogWarning($"Архив: Плашка с ID '{id}' не найдена!");
    }

    public void RefreshVisuals()
    {
        bool allPrevDone = true; // Для логики открытия плашек по цепочке
        bool currentStageFinished = true; // Для логики кнопки "Вперед"

        for (int s = 0; s < stages.Count; s++)
        {
            var stage = stages[s];
            bool thisStageIsActuallyFinished = true;

            foreach (var plate in stage.plates)
            {
                bool isPlateDone = plate.leftDone && plate.rightDone;

                // Плашка видна только если все предыдущие в игре пройдены
                plate.uiElement.gameObject.SetActive(allPrevDone);
                plate.uiElement.SetState(plate.leftDone, plate.rightDone, allPrevDone);

                if (!isPlateDone)
                {
                    thisStageIsActuallyFinished = false;
                    allPrevDone = false;
                }
            }

            // Если мы сейчас обсчитываем именно ТОТ этап, на котором стоит игрок
            if (s == activeStageIndex)
            {
                currentStageFinished = thisStageIsActuallyFinished;
            }
        }

        // --- УПРАВЛЕНИЕ ГЛОБАЛЬНЫМИ КНОПКАМИ ---

        // Назад: Видна всегда, кроме первого этапа
        if (globalPrevButton != null)
            globalPrevButton.gameObject.SetActive(activeStageIndex > 0);

        // Вперед: Видна если этап пройден И это не последний этап
        if (globalNextButton != null)
            globalNextButton.gameObject.SetActive(currentStageFinished && activeStageIndex < stages.Count - 1);
    }

    // --- DEBUG КНОПКИ ---

    public void DebugSkipNext()
    {
        foreach (var stage in stages)
        {
            foreach (var plate in stage.plates)
            {
                if (!plate.leftDone) { plate.leftDone = true; RefreshVisuals(); return; }
                if (!plate.rightDone) { plate.rightDone = true; RefreshVisuals(); return; }
            }
        }
    }

    public void DebugResetArchive()
    {
        foreach (var stage in stages)
        {
            foreach (var plate in stage.plates)
            {
                plate.leftDone = false;
                plate.rightDone = false;
            }
        }
        RefreshVisuals();
        saveLoadManager.instance.saveGame();
    }

    // --- СОХРАНЕНИЕ ---

    public void save(ref saveData data)
    {
        data.ArchiveProgress.Clear();
        foreach (var stage in stages)
        {
            foreach (var plate in stage.plates)
            {
                data.ArchiveProgress.Add(new PlateSaveInfo { l = plate.leftDone, r = plate.rightDone });
            }
        }
    }

    public void load(saveData data)
    {
        if (data.ArchiveProgress == null || data.ArchiveProgress.Count == 0) return;
        int index = 0;
        foreach (var stage in stages)
        {
            foreach (var plate in stage.plates)
            {
                if (index < data.ArchiveProgress.Count)
                {
                    plate.leftDone = data.ArchiveProgress[index].l;
                    plate.rightDone = data.ArchiveProgress[index].r;
                    index++;
                }
            }
        }
        RefreshVisuals();
    }
}

[System.Serializable]
public class PlateSaveInfo { public bool l; public bool r; }