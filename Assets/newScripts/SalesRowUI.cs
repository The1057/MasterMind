using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Вешается на префаб строки в экране продажи
public class SalesRowUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text soldText;     // продано за прошлый месяц
    public TMP_Text shareText;    // доля от общей выручки
    public TMP_Text priceText;    // текущая цена продажи (необязательно)
    public TMP_Text stockText;    // остаток на складе (необязательно)
    public Button editButton;     // карандаш

    public void setup(Item item, float totalRevenue, Action<Item> onEdit)
    {
        nameText.text = item.name;
        soldText.text = $"Продано: {item.soldLastMonth:0} шт.";
        float share = totalRevenue > 0 ? item.revenueLastMonth / totalRevenue * 100f : 0f;
        shareText.text = "(" + share.ToString("0.#") + "%)";
        if (priceText != null) priceText.text = item.selling_price.ToString("0.##");
        if (stockText != null) stockText.text = item.stock + " шт.";

        bool hasStock = item.stock >= 1;
        editButton.gameObject.SetActive(hasStock); // или interactable, если хотите оставить видимой
        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(() => onEdit(item));
    }
}
