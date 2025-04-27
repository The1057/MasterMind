using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class moneyData
{
    public float player_money = 0;

    public List<storeScript> storeList;
    public float totalIncome = 0, totalExpense = 0;

    public float profit;
    public float operationProfit;

}
