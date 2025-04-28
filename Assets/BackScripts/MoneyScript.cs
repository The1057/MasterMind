using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MoneyScript : MonoBehaviour, ISaveLoadable, ITickable
{
    float player_money = 0;

    public List<storeScript> storeList;
    float totalIncome=0, totalExpense=0;
    public float tax1 = 1;
    public float tax2 = 0.87f;

    public float profit;
    public float operationProfit;
        public void save(ref saveData saveData)
    {
        saveData.MoneyData.operationProfit = operationProfit;
        saveData.MoneyData.storeList = storeList;
        saveData.MoneyData.profit = profit;
        saveData.MoneyData.player_money = player_money;
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
        storeList = FindObjectsByType<storeScript>(FindObjectsSortMode.InstanceID).ToList();
        foreach (var store in storeList)
        {
            totalIncome += store.countIncome();
            totalExpense += store.countExpense();
        }
        profit = tax1 * (tax2 * totalIncome - totalExpense) + operationProfit;

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
    }
    public float getMoney()
    {
        return player_money;
    }
}
