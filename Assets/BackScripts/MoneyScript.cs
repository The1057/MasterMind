using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    float player_money = 0;

    public List<storeScript> storeList;
    float totalIncome=0, totalExpense=0;
    public float tax1 = 1;
    public float tax2 = 0.87f;

    public float profit;
    public float operationProfit;

    public ClockScript clock;
    
    void Start()
    {
        clock = GameObject.FindGameObjectWithTag("ClockTag").GetComponent<ClockScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (clock.isNextTurn())
        {
            profit = 0;
            totalIncome = 0;
            totalExpense = 0;
            foreach (var store in storeList)
            {
                totalIncome += store.countIncome();
                totalExpense += store.countExpense();
            }
            profit = tax1 * (tax2*totalIncome - totalExpense) + operationProfit;
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
    }
    public float getMoney()
    {
        return player_money;
    }
}
