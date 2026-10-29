using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Кнопка "Запустить": прокручивает месяц
public class NextTurnButton : MonoBehaviour
{
    public ClockScript clock;
    public Button button;
    public TMP_Text dateText;   // необязательно: "Месяц 3, год 1"
    public UnityEvent onTurnProcessed; // сюда можно повесить purchaseScreen.refresh и salesScreen.refresh

    void Awake() { button.onClick.AddListener(() => StartCoroutine(run())); }
    void OnEnable() { updateDate(); }

    IEnumerator run()
    {
        button.interactable = false;
        clock.forceTurn();                    // Clock обработает в своём Update
        yield return new WaitForEndOfFrame(); // к этому моменту Update уже прошёл
        updateDate();
        onTurnProcessed?.Invoke();
        button.interactable = true;
    }

    void updateDate()
    {
        if (dateText != null) dateText.text = $"Время в игре: месяц - {clock.getMonth()}, год - {clock.getYear()}";
    }
}
