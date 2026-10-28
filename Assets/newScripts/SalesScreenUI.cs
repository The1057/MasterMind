using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

// Экран продажи
public class SalesScreenUI : MonoBehaviour
{
    public MoneyScript moneyScript;
    public int storeIndex = 0;

    public Transform listParent;
    public SalesRowUI rowPrefab;
    public PriceWindowUI priceWindow;
    public TMP_Text moneyText;              // необязательно
    public BusinessTableAnimated table;     // необязательно

    readonly List<SalesRowUI> rows = new();

    void OnEnable() { refresh(); }

    public void refresh()
    {
        var store = moneyScript.getStore(storeIndex);
        if (store == null) return;

        foreach (var r in rows)
        {
            if (table != null) table.rows.Remove(r.GetComponent<RectTransform>());
            Destroy(r.gameObject);
        }
        rows.Clear();

        float totalRevenue = store.items.Sum(i => i.revenueLastMonth);

        foreach (var item in store.items)
        {
            var row = Instantiate(rowPrefab, listParent);
            row.gameObject.SetActive(true);
            row.setup(item, totalRevenue, it => priceWindow.open(store, it, refresh));
            rows.Add(row);
            if (table != null) table.rows.Add(row.GetComponent<RectTransform>());
        }
        if (table != null) table.setupTable();
        if (moneyText != null) moneyText.text = moneyScript.getMoney().ToString("0.##");
    }
}
