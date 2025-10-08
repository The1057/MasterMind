using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        this.operationProfit = loadData.MoneyData.operationProfit;
        this.storeList = loadData.MoneyData.storeList;
        this.profit = loadData.MoneyData.profit;
        this.player_money = loadData.MoneyData.player_money;
        this.player_gems = loadData.MoneyData.player_gems;
        this.totalExpense = loadData.MoneyData.totalExpense;
        this.totalIncome = loadData.MoneyData.totalIncome;
        this.tax1 = loadData.MoneyData.tax1;
        this.tax2 = loadData.MoneyData.tax2;

    }

    public void nextTurn(int month, int year)
    {
        profit = 0;
        totalIncome = 0;
        totalExpense = 0;
        float constExpenses = 0;
        storeList = FindObjectsByType<storeScript>(FindObjectsSortMode.InstanceID).ToList();
        foreach (var store in storeList)
        {
            totalIncome += store.countIncome();
            totalExpense += store.countExpense();
            constExpenses += store.constExpense;
        }
        profit = tax1 * (tax2 * totalIncome - totalExpense) + operationProfit;
        var taxExpense = profit - ((totalIncome - totalExpense) + operationProfit);

        addMoney(profit);

        statisticsScript.statistics.expenceStat[month - 1] = totalExpense;
        statisticsScript.statistics.incomeStat[month - 1] = totalIncome;
        statisticsScript.statistics.profitStat[month-1] = profit;
        statisticsScript.statistics.taxExpenseStat[month-1] = taxExpense;
        statisticsScript.statistics.constExpenseStat[month - 1] = constExpenses;
        if ((profit * 100) / totalIncome == Mathf.Infinity)
        {
            statisticsScript.statistics.ROSStat[month - 1] = 100;
        }
        else if((profit * 100) / totalIncome == Mathf.NegativeInfinity)
        {
            statisticsScript.statistics.ROSStat[month - 1] = -100;
        }
        else
        {
            statisticsScript.statistics.ROSStat[month - 1] = (profit * 100) / totalIncome;
        }
    }
    public bool setMoney(float moneyAmount)
    {
        if (moneyAmount >= 0)
        {
            player_money = moneyAmount;
            return true;
        } else
        {
            player_money = 0;
            return false;
        }
    }
    public void addMoney(float moneyAmount)
    {
        player_money += moneyAmount;
        if(player_money < 0)
        {
            player_money = 0;
            Debug.Log("В долги уходишь, друг");
        }
    }
    public float getMoney()
    {
        return player_money;
    }
}
