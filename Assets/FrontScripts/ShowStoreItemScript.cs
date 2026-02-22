using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ShowStoreItemScript : MonoBehaviour
{
    [Header("Required objects, store")]
    public Transform itemList;
    public GameObject itemPrefab;
    public MoneyScript moneyScript;
    public storeScript currentStore;
    public BusinessTableAnimated BusinessTableAnimated;

    [Header("Required objects, warehouse")]
    public Transform warehouseItemList;
    public ShopItemRedactScript ShopItemRedactScript;
    public GameObject WHitemPrefab;
    public GameObject itemBlocker;

    List<Item> avilableItems = new();
    List<GameObject> currentItems = new List<GameObject>();
    List<GameObject> currentWarehouseItems = new List<GameObject>();
    public void displayItems()
    {
        foreach (var item in currentItems)
        {
            Destroy(item);
            BusinessTableAnimated.rows.Remove(item.GetComponent<RectTransform>());
        }
        currentItems.Clear();

        int i = 0;
        foreach (var item in currentStore.items)
        {
            GameObject newItem = Instantiate(itemPrefab, itemList);
            currentItems.Add(newItem);

            StoreItemUI itemUI = newItem.GetComponent<StoreItemUI>();
            if (itemUI == null)
            {
                Debug.LogError("На префабе отсутствует компонент StoreItemUI!");
                continue;
            }

            // Устанавливаем название
            itemUI.itemNameText.text = item.name;

            // Устанавливаем текущие значения
            itemUI.amountInput.text = item.bought_number.ToString();
            itemUI.priceInput.text = item.selling_price.ToString();

            // Очищаем старые слушатели, чтобы не накапливались (важно!)
            itemUI.amountInput.onEndEdit.RemoveAllListeners();
            itemUI.priceInput.onEndEdit.RemoveAllListeners();
            // Также можно очистить onDeselect, если используете

            // Добавляем новые слушатели с захватом индекса элемента
            int index = i; // или захватывайте item, если хотите работать с объектом напрямую
            itemUI.amountInput.onEndEdit.AddListener((value) => {
                float.TryParse(value, out currentStore.items[index].bought_number);
            });
            itemUI.priceInput.onEndEdit.AddListener((value) => {
                float.TryParse(value, out currentStore.items[index].selling_price);
            });

            // Добавляем в таблицу анимации
            BusinessTableAnimated.rows.Add(newItem.GetComponent<RectTransform>());
            i++;
        }

        BusinessTableAnimated.setupTable();
    }
    public void setStore(int storeId)
    {
        currentStore = moneyScript.storeList[storeId];
    }
    public void displayWarehouseItems()
    {
        foreach (var item in currentWarehouseItems)
        {
            Destroy(item);
        }

        avilableItems = ShopItemRedactScript.getPossibleItems();

        foreach (var item in avilableItems)
        {
            if (!isInStore(item))
            {
                var lastItem = Instantiate(WHitemPrefab, warehouseItemList);
                lastItem.GetComponentsInChildren<TextMeshProUGUI>()[0].text = item.name;
                lastItem.GetComponentsInChildren<TextMeshProUGUI>()[2].text = item.demand_min.ToString();
                lastItem.GetComponentsInChildren<TextMeshProUGUI>()[3].text = item.buying_price.ToString();
                lastItem.GetComponent<Button>().onClick.AddListener(() => { ShopItemRedactScript.addItemById(item.id); blockItem(lastItem); });
                currentWarehouseItems.Add(lastItem);
            }
        }
    }

    void blockItem(GameObject item)
    {
        var blocker = Instantiate(itemBlocker, item.transform);
        item.GetComponent<Button>().interactable = false;
    }
    bool isInStore(Item item)
    {
        foreach(var item2 in currentStore.items)
        {
            if (item2.name == item.name) return true;
        }
        return false;
    }
}
