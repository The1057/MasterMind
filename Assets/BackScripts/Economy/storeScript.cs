using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class storeScript : MonoBehaviour, ISaveLoadable
{
    // enum оставлен, потому что на него ссылается RivalBizScript
    public enum demandCalcMethod
    {
        randomNormalDistribution = 0,
        randomLinearDistribution = 1,
        linear = 2
    }

    public struct MonthResult
    {
        public float income;     // выручка
        public float expense;    // закупки + постоянные + случайные расходы
        public float purchases;  // сколько уже заплачено за закупки (деньги списаны раньше)
    }

    public int storeId = 0;
    public List<Item> items = new List<Item>();

    [Header("Реклама")]
    public float gloabalAdModifier = 1;
    public float localAdModifier = 0;
    public float adModifier = 1f;

    [Header("Расходы")]
    public float constExpense = 1000;
    public float randomExpenseMin = 0.03f;
    public float randomExpenseMax = 0.05f;

    [Header("Спрос")]
    [Range(0f, 0.5f)] public float demandNoise = 0.05f; // случайный разброс спроса +-5%
    public float demandChangeDeviation = 5;             // оставлено для совместимости с сейвом

    [Header("Состояние месяца")]
    public float purchasesThisMonth = 0;

    // ---------- Сохранение ----------
    public void save(ref saveData data)
    {
        while (data.StoreDatas.Count <= storeId) data.StoreDatas.Add(new storeData());

        var d = data.StoreDatas[storeId];
        d.adModifier = adModifier;
        d.items = items;
        d.storeID = storeId;
        d.name = name;
        d.constExpense = constExpense;
        d.randomExpenseMax = randomExpenseMax;
        d.randomExpenseMin = randomExpenseMin;
        d.demandChangeDeviation = demandChangeDeviation;
        d.purchasesThisMonth = purchasesThisMonth; // НОВОЕ ПОЛЕ в storeData
    }

    public void load(saveData data)
    {
        if (storeId >= data.StoreDatas.Count) return;
        var d = data.StoreDatas[storeId];
        adModifier = d.adModifier;
        constExpense = d.constExpense;
        randomExpenseMin = d.randomExpenseMin;
        randomExpenseMax = d.randomExpenseMax;
        demandChangeDeviation = d.demandChangeDeviation;
        items = d.items ?? new List<Item>();
        name = d.name;
        purchasesThisMonth = d.purchasesThisMonth;
    }

    // ---------- Поиск ----------
    public Item findItem(string itemName)
    {
        return items.FirstOrDefault(i => i.name == itemName);
    }

    // ---------- Закупка ----------
    public int getBuyMin(Item template)
    {
        return Mathf.Max(1, Mathf.RoundToInt(template.demand_min));
    }

    public int getBuyMax(Item template, float playerMoney)
    {
        int max = Mathf.Max(getBuyMin(template), Mathf.RoundToInt(template.demand_max));
        int affordable = template.buying_price > 0
            ? Mathf.FloorToInt(playerMoney / template.buying_price)
            : max;
        return Mathf.Min(max, affordable); // может быть 0, если денег не хватает
    }

    public bool buy(Item template, int qty, MoneyScript money)
    {
        if (qty <= 0) return false;
        float cost = template.buying_price * qty;
        if (!money.trySpend(cost)) return false;

        var item = findItem(template.name);
        if (item == null)
        {
            item = template.Clone(); // КЛОН, а не общий объект из possibleItems
            item.stock = 0;
            item.soldLastMonth = 0;
            item.revenueLastMonth = 0;
            item.selling_price = Mathf.Clamp(item.selling_price,
                item.selling_price_min, item.selling_price_max);
            if (item.selling_price <= 0)
                item.selling_price = (item.selling_price_min + item.selling_price_max) / 2f;
            items.Add(item);
        }
        item.stock += qty;
        purchasesThisMonth += cost;
        return true;
    }

    // ---------- Цена продажи ----------
    public bool isPriceValid(Item item, float price)
    {
        return price >= item.selling_price_min && price <= item.selling_price_max;
    }

    public bool setPrice(Item item, float price)
    {
        if (!isPriceValid(item, price)) return false;
        item.selling_price = price;
        return true;
    }

    // ---------- Симуляция месяца ----------
    public float calcDemand(Item item)
    {
        float pMin = item.selling_price_min, pMax = item.selling_price_max;
        if (Mathf.Approximately(pMin, pMax)) return item.demand_max;
        // та же прямая, что давала ваша формула: (pMin, demand_max) -> (pMax, demand_min)
        float t = Mathf.InverseLerp(pMin, pMax, item.selling_price);
        return Mathf.Lerp(item.demand_max, item.demand_min, t);
    }

    public MonthResult simulateMonth()
    {
        adModifier = gloabalAdModifier + localAdModifier;
        float income = 0;

        foreach (var item in items)
        {
            item.selling_price = Mathf.Clamp(item.selling_price,
                item.selling_price_min, item.selling_price_max);

            float demand = calcDemand(item) * adModifier
                           * (1f + Random.Range(-demandNoise, demandNoise));
            int wanted = Mathf.Max(0, Mathf.RoundToInt(demand));
            int sold = Mathf.Min(item.stock, wanted);

            item.stock -= sold;
            item.soldLastMonth = sold;
            item.sold_number = sold; // совместимость со старым кодом
            item.revenueLastMonth = sold * item.selling_price;
            income += item.revenueLastMonth;
        }

        var res = new MonthResult();
        res.income = income;
        res.purchases = purchasesThisMonth;
        res.expense = purchasesThisMonth + constExpense
                      + income * Random.Range(randomExpenseMin, randomExpenseMax);
        purchasesThisMonth = 0;
        return res;
    }
}