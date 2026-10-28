using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Экран закупки. Вешается на канвас закупки (или на объект внутри него).
public class PurchaseScreenUI : MonoBehaviour
{
    public MoneyScript moneyScript;
    public ShopItemRedactScript shopItemRedact; // оттуда берём getPossibleItems()
    public int storeIndex = 0;

    public Transform listParent;
    public PurchaseRowUI rowPrefab;
    public BuyWindowUI buyWindow;
    public TMP_Text moneyText;                  // необязательно
    public BusinessTableAnimated table;         // необязательно

    readonly List<PurchaseRowUI> rows = new();

    void OnEnable() { refresh(); }

    public void refresh()
    {
        var store = moneyScript.getStore(storeIndex);
        
        var possible = shopItemRedact.getPossibleItems();
        
        if (store == null) return;

        foreach (var r in rows)
        {
            if (table != null) table.rows.Remove(r.GetComponent<RectTransform>());
            Destroy(r.gameObject);
        }
        rows.Clear();

        if (possible != null)
        {
            foreach (var template in possible)
            {
                var inStore = store.findItem(template.name);
                var row = Instantiate(rowPrefab, listParent);
                row.gameObject.SetActive(true);
                row.setup(template, inStore != null ? inStore.stock : 0,
                    t => buyWindow.open(store, moneyScript, t, refresh));
                rows.Add(row);
                if (table != null) table.rows.Add(row.GetComponent<RectTransform>());
            }
        }
        if (table != null) table.setupTable();
        if (moneyText != null) moneyText.text = moneyScript.getMoney().ToString("0.##");
    }
}
