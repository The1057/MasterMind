using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Окошко изменения цены продажи
public class PriceWindowUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text rangeText;
    public TMP_Text errorText;   // необязательно
    public TMP_InputField priceInput;
    public Button saveButton;
    public Button closeButton;

    storeScript store;
    Item item;
    Action onChanged;

    void Awake()
    {
        priceInput.contentType = TMP_InputField.ContentType.DecimalNumber;
        saveButton.onClick.AddListener(onSave);
        if (closeButton != null) closeButton.onClick.AddListener(close);
    }

    public void open(storeScript store, Item item, Action onChanged)
    {
        this.store = store;
        this.item = item;
        this.onChanged = onChanged;

        titleText.text = item.name;
        rangeText.text = $"Введите цену:\n от {item.selling_price_min:0.##} до {item.selling_price_max:0.##}";
        priceInput.text = item.selling_price.ToString("0.##");
        if (errorText != null) errorText.text = "";
        gameObject.SetActive(true);
    }

    void onSave()
    {
        string s = priceInput.text.Replace(',', '.');
        if (float.TryParse(s, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float price)
            && store.setPrice(item, price))
        {
            onChanged?.Invoke();
            close();
        }
        else if (errorText != null)
        {
            errorText.text = $"Введите число от {item.selling_price_min:0.##} до {item.selling_price_max:0.##}";
        }
    }

    public void close() { gameObject.SetActive(false); }
}
