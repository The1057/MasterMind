using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemUI : MonoBehaviour
{
    public TextMeshProUGUI amountInput;      // поле "Кол-во ввода"
    public TextMeshProUGUI priceInput;        // поле "Цена ввода"
    public TextMeshProUGUI itemNameText;     // текст с названием товара

    public GameObject windowItemInWH;

    public GameObject windowItemInStore;
    public void showWIIS()
    {
        windowItemInStore.SetActive(true);
    }
    public void showWIIWH()
    {
        windowItemInWH.SetActive(true);
    }
    public void closeWIIS()
    {
        windowItemInStore.SetActive(false);
    }
    public void closeWIIWH()
    {
        windowItemInWH.SetActive(false);
    }
}