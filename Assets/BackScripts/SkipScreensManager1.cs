using UnityEngine;
using System.Collections;

public class SkipScreensManager1 : MonoBehaviour
{
    public playerDataClass playerDataObject; // Ссылка на компонент с данными
    public CanvasSwitcher1 canvasSwitcher; // Ссылка на твой переключатель

    [Header("Канвасы")]
    public GameObject initialCanvas2;      // То, что выключаем (заставка)
    public GameObject profileCanvas;       // Куда идем, если данные ЕСТЬ
    public GameObject registrationCanvas;  // Куда идем, если данных НЕТ

    IEnumerator Start()
    {
        // 1. Ждем чуть-чуть, чтобы система сохранения успела прогнать метод Load()
        // Если загрузка идет из файла, 0.1 сек обычно достаточно.
        yield return new WaitForSeconds(0.1f);

        // 2. Проверяем данные
        if (playerDataObject == null || playerDataObject.data == null)
        {
            Debug.LogError("Данные игрока не найдены!");
            Switch(registrationCanvas);
            yield break;
        }

        // Проверяем заполненность профиля
        bool hasGender = !string.IsNullOrEmpty(playerDataObject.data.player_gender);
        bool hasName = !string.IsNullOrEmpty(playerDataObject.data.player_name);

        if (hasGender && hasName)
        {
            Debug.Log("Профиль заполнен, идем в Profile");
            Switch(profileCanvas);
        }
        else
        {
            Debug.Log("Профиль пуст, идем на Регистрацию");
            Switch(registrationCanvas);
        }
    }

    void Switch(GameObject target)
    {
        if (target == null) return;

        // Вместо простого SetActive, вызываем метод у твоего Switcher, 
        // чтобы он зафиксировал изменение в своей логике (lastCanvases и т.д.)
        if (canvasSwitcher != null)
        {
            canvasSwitcher.SwitchToCanvas(target);
        }
        else
        {
            target.SetActive(true);
        }

        if (initialCanvas2 != null) initialCanvas2.SetActive(false);
    }
}