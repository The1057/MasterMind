using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Вешается на префаб строки в экране закупки
public class PurchaseRowUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text stockText;   // сколько на складе
    public TMP_Text priceText;   // цена закупки
    public Button plusButton;

    public void setup(Item template, int stock, Action<Item> onPlus)
    {
        nameText.text = template.name;
        stockText.text = stock + " шт.";
        priceText.text = template.buying_price.ToString("0.##");
        plusButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(() => onPlus(template));
    }
}
