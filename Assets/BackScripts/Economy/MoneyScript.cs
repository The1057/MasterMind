using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoneyScript : MonoBehaviour, ISaveLoadable, ITickable
{
    public float player_money = 0;
    public int player_gems = 0;
    [Header("Objects")]
    public statisticsScript statisticsScript;
    public List<storeScript> storeList;

    [Header("Economy Parameters")]
    public float tax1 = 1;
    public float tax2 = 0.87f;
    [Header("Output data")]
    public float profit;
    public float operationProfit;

    float totalIncome = 0, totalExpense = 0;

    public void save(ref saveData saveData)
    {
        saveData.MoneyData.operationProfit = operationProfit;
        saveData.MoneyData.storeList = storeList;
        saveData.MoneyData.profit = profit;
        saveData.MoneyData.player_money = player_money;
        saveData.MoneyData.player_gems = player_gems;
        saveData.MoneyData.totalExpense = totalExpense;
        saveData.MoneyData.totalIncome = totalIncome;
        saveData.MoneyData.tax1 = tax1;
        saveData.MoneyData.tax2 = tax2;
    }

    public void load(saveData loadData)
    {
        operationProfit = loadData.MoneyData.operationProfit;
        storeList = FindObjectsByType<storeScript>(FindObjectsSortMode.None)
            .OrderBy(a => a.storeId).ToList();
        profit = loadData.MoneyData.profit;
        player_money = loadData.MoneyData.player_money;
        player_gems = loadData.MoneyData.player_gems;
        totalExpense = loadData.MoneyData.totalExpense;
        totalIncome = loadData.MoneyData.totalIncome;
        tax1 = loadData.MoneyData.tax1;
        tax2 = loadData.MoneyData.tax2;
    }

    public storeScript getStore(int index)
    {
        if (storeList == null || storeList.Count <= index || storeList[index] == null)
        {
            storeList = FindObjectsByType<storeScript>(FindObjectsSortMode.None)
                .OrderBy(s => s.storeId).ToList();
        }
        return index < storeList.Count ? storeList[index] : null;
    }

    public void nextTurn(int month, int year)
    {
        storeList = FindObjectsByType<storeScript>(FindObjectsSortMode.None)
            .OrderBy(s => s.storeId).ToList();

        totalIncome = 0;
        totalExpense = 0;
        float constExpenses = 0;
        float purchasesAlreadyPaid = 0;

        foreach (var store in storeList)
        {
            var r = store.simulateMonth();
            totalIncome += r.income;
            totalExpense += r.expense;
            purchasesAlreadyPaid += r.purchases;
            constExpenses += store.constExpense;
        }

        profit = tax1 * (tax2 * totalIncome - totalExpense) + operationProfit;
        float taxExpense = profit - ((totalIncome - totalExpense) + operationProfit);

        // Деньги за закупки списаны сразу при покупке, а в profit они уже учтены как расход.
        // Чтобы не вычесть их дважды, возвращаем их в сумму.
        addMoney(profit + purchasesAlreadyPaid);

        var st = statisticsScript.statistics;
        st.expenceStat[month - 1] = totalExpense;
        st.incomeStat[month - 1] = totalIncome;
        st.profitStat[month - 1] = profit;
        st.taxExpenseStat[month - 1] = taxExpense;
        st.constExpenseStat[month - 1] = constExpenses;

        if (totalIncome > 0) st.ROSStat[month - 1] = profit * 100f / totalIncome;
        else st.ROSStat[month - 1] = profit > 0 ? 100 : (profit < 0 ? -100 : 0);
    }

    public bool trySpend(float amount)
    {
        if (amount > player_money) return false;
        player_money -= amount;
        return true;
    }

    public bool setMoney(float moneyAmount)
    {
        if (moneyAmount >= 0) { player_money = moneyAmount; return true; }
        player_money = 0;
        return false;
    }

    public void addMoney(float moneyAmount)
    {
        player_money += moneyAmount;
        if (player_money < 0)
        {
            player_money = 0;
            Debug.Log("Игрок в минусе, деньги обнулены");
        }
    }

    public float getMoney() { return player_money; }
}