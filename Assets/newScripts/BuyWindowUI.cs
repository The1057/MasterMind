using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Окошко покупки (закрывается после покупки)
public class BuyWindowUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text rangeText;     // "Можно купить: от X до Y"
    public TMP_Text totalText;     // "Итого: ..."
    public TMP_Text errorText;     // можно оставить пустым
    public TMP_InputField amountInput;
    public Button buyButton;
    public Button closeButton;

    storeScript store;
    MoneyScript money;
    Item template;
    Action onBought;
    int min, max;

    void Awake()
    {
        amountInput.contentType = TMP_InputField.ContentType.IntegerNumber;
        amountInput.onValueChanged.AddListener(_ => updateTotal());
        amountInput.onEndEdit.AddListener(_ => clampInput());
        buyButton.onClick.AddListener(onBuyClicked);
        if (closeButton != null) closeButton.onClick.AddListener(close);
    }

    public void open(storeScript store, MoneyScript money, Item template, Action onBought)
    {
        this.store = store;
        this.money = money;
        this.template = template;
        this.onBought = onBought;

        max = store.getBuyMax(template, money.getMoney());
        min = Mathf.Min(store.getBuyMin(template), max);

        titleText.text = template.name;
        if (errorText != null) errorText.text = "";
        rangeText.text = max <= 0 ? "Недостаточно денег" : $"Можно купить:\nот {min} до {max}";
        buyButton.interactable = max > 0;
        amountInput.text = max > 0 ? min.ToString() : "0";

        gameObject.SetActive(true);
        updateTotal();
    }

    int parse()
    {
        return int.TryParse(amountInput.text, out int v) ? v : 0;
    }

    void clampInput()
    {
        if (max <= 0) return;
        amountInput.text = Mathf.Clamp(parse(), min, max).ToString();
    }

    void updateTotal()
    {
        if (template == null) return;
        totalText.text = "Итого: " + (parse() * template.buying_price).ToString("0.##");
    }

    void onBuyClicked()
    {
        int qty = parse();
        if (qty < min || qty > max)
        {
            if (errorText != null) errorText.text = $"Введите число от {min} до {max}";
            return;
        }
        if (store.buy(template, qty, money))
        {
            onBought?.Invoke();
            close();
        }
        else if (errorText != null) errorText.text = "Не удалось купить";
    }

    public void close() { gameObject.SetActive(false); }
}
