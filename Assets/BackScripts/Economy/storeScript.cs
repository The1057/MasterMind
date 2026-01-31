using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class storeScript : MonoBehaviour, ISaveLoadable, ITickable
{
    public enum demandCalcMethod
    {
        randomNormalDistribution = 0,
        randomLinearDistribution = 1,
        linear = 2
    }


    public int storeId = 0;

    public List<Item> items = new List<Item>();

    public float gloabalAdModifier = 1;
    public float localAdModifier = 0;
    public float adModifier = 1.1f;
    public float constExpense = 1000;//постоянные затраты: аренда, зарплата
    public float randomExpenseMin = 0.03f;
    public float randomExpenseMax = 0.05f;
    public float demandChangeDeviation = 5;
    public void save(ref saveData data)
    {
        if(data.StoreDatas.Count <= storeId)
        {
            data.StoreDatas.Add(new storeData());
        }
        data.StoreDatas[storeId].adModifier = adModifier;
        data.StoreDatas[storeId].items = items;
        data.StoreDatas[storeId].storeID = storeId;
        data.StoreDatas[storeId].name = name;
        data.StoreDatas[storeId].constExpense = constExpense;
        data.StoreDatas[storeId].randomExpenseMax = randomExpenseMax;
        data.StoreDatas[storeId].randomExpenseMin = randomExpenseMin;
        data.StoreDatas[storeId].demandChangeDeviation = demandChangeDeviation;
    }
    public void load(saveData data)
    {
        this.adModifier = data.StoreDatas[storeId].adModifier;
        this.constExpense = data.StoreDatas[storeId].constExpense;
        this.randomExpenseMin = data.StoreDatas[storeId].randomExpenseMin;
        this.randomExpenseMax = data.StoreDatas[storeId].randomExpenseMax;
        this.demandChangeDeviation = data.StoreDatas[storeId].demandChangeDeviation;
        this.items = data.StoreDatas[storeId].items;        
        this.name = data.StoreDatas[storeId].name;
    }
    public void nextTurn(int month, int year)
    {
        for (int i = 0; i < items.Count; i++)
        {
            recalculateDemand(items[i],demandChangeDeviation,demandCalcMethod.linear);
            //print($"New demand for {items[i].name} is {items[i].bought_number}");
        }
    }

    void Start()
    {
        //addDebug();
    }

    [ContextMenu("Add debug item")]
    public void addDebug()
    {
        items.Add(breadItemList.possibleItems.First());
        items.Last().selling_price = 100;
        items.Last().bought_number = 400;
    }
    public float countIncome()
    {
        float res = 0;
        adModifier = gloabalAdModifier + localAdModifier;
        foreach (var item in items)
        {
            if (item.sold_number <= item.bought_number)
            {
                res += item.selling_price * item.sold_number * adModifier;
            }
            else
            {
                res += item.selling_price * item.bought_number * adModifier;
            }
        }

        return res;
    }
    public float countExpense()
    {
        float res = 0;

        foreach (var item in items)
        {
            res += item.buying_price * item.bought_number;
        }
        res += constExpense;
        res -= countIncome()*getRandomExpense();
        return res;
    }
    public void recalculateDemand(Item item,float deviation, demandCalcMethod calcMethod)
    {        
        System.Random random = new System.Random();
        switch (calcMethod)
        {
            case demandCalcMethod.randomNormalDistribution:

                double randomsSum = 0;
                for(int i = 0; i < 12; i++)
                {
                    randomsSum += random.NextDouble();
                }
                randomsSum -= 6;
                item.sold_number = (float)randomsSum * deviation + item.sold_number;
                
                break;
            case demandCalcMethod.randomLinearDistribution:

                item.sold_number = (item.sold_number - deviation) * (float)random.NextDouble() + item.sold_number;

                break;
            case demandCalcMethod.linear:

                float yMean = (item.demand_max + item.demand_min) / 2f;
                float xMean = (item.selling_price_max + item.selling_price_min) / 2f;
                float m = (((item.selling_price_max - xMean) * (item.demand_min - yMean)) + ((item.selling_price_min - xMean) * (item.demand_max - yMean))) /
                    (MathF.Pow(item.selling_price_max - xMean, 2f) + MathF.Pow(item.selling_price_min - xMean, 2f));
                float b = yMean - m * xMean;
                item.sold_number = m * item.selling_price + b;
                break;
            default:

                break;

        }
        if (item.sold_number < 0)
        {
            item.sold_number = 0;
        }
    }
    public float getRandomExpense()
    {
        float res=0;

        res = UnityEngine.Random.Range(randomExpenseMin,randomExpenseMax);

        return res;
    }

    [ContextMenu("Print first item")]
    public void showDebug()
    {
        
    }
}