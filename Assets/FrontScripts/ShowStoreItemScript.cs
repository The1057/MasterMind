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

        var plusButton = BusinessTableAnimated.rows.Last();
        BusinessTableAnimated.rows.Remove(plusButton);
        int i = 0;
        foreach (var item in currentStore.items)
        {
            currentItems.Add(Instantiate(itemPrefab,itemList));
            currentItems.Last().GetComponent<TextMeshProUGUI>().text = item.name;
            currentItems.Last().transform.GetChild(0).GetComponent<TMP_InputField>().text = item.bought_number.ToString();
            currentItems.Last().transform.GetChild(1).GetComponent<TMP_InputField>().text = item.selling_price.ToString();

            currentItems.Last().transform.GetChild(0).GetComponent<TMP_InputField>().onEndEdit.AddListener((call) => { int i = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetSiblingIndex() - 5; ; float.TryParse(call, out currentStore.items[i].bought_number); });
            currentItems.Last().transform.GetChild(1).GetComponent<TMP_InputField>().onEndEdit.AddListener((call) => { int i = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.parent.GetSiblingIndex() - 5; ; float.TryParse(call, out currentStore.items[i].selling_price); });
            
            BusinessTableAnimated.rows.Add(currentItems.Last().GetComponent<RectTransform>());

        }
        BusinessTableAnimated.rows.Add(plusButton);

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
                lastItem.GetComponent<TextMeshProUGUI>().text = item.name;
                lastItem.GetComponentsInChildren<TextMeshProUGUI>()[1].text = item.demand_min.ToString();
                lastItem.GetComponentsInChildren<TextMeshProUGUI>()[2].text = item.buying_price.ToString();
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
