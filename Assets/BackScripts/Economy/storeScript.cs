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
    
    public float adModifier = 1.1f;
    public float rent = 1000;
    public float randomExpenseMin = 0.03f;
    public float randomExpenseMax = 0.05f;
    public float demandChangeDeviation = 5;

    public List<Vector2> points4Graph = Enumerable.Repeat(new Vector2(1, 0), 12).ToList();
    public List<Vector2> points4GraphFinal  = Enumerable.Repeat(new Vector2(0, 0), 12).ToList();

    public void save(ref saveData data)
    {
        if(data.StoreDatas.Count <= storeId)
        {
            data.StoreDatas.Add(new storeData());
        }
        data.StoreDatas[storeId].adModifier = adModifier;
        data.StoreDatas[storeId].items = items;
        data.StoreDatas[storeId].points4Graph = points4Graph;
        data.StoreDatas[storeId].points4GraphFinal = points4GraphFinal;
        data.StoreDatas[storeId].storeID = storeId;
        data.StoreDatas[storeId].name = name;
    }
    public void load(saveData data)
    {
        this.adModifier = data.StoreDatas[storeId].adModifier;
        this.items = data.StoreDatas[storeId].items;
        //this.items = new List<Item>();
        //foreach (var item in data.StoreDatas[storeId].items)
        //{
        //    this.items.Add(item);
        //}
        this.points4Graph = data.StoreDatas[storeId].points4Graph;
        this.points4GraphFinal = data.StoreDatas[storeId].points4GraphFinal;
        this.name = data.StoreDatas[storeId].name;
    }
    public void nextTurn(int month, int year)
    {
        if (month == 1)
        {
            points4GraphFinal = new List<Vector2>(points4Graph);
        }
        points4Graph[month - 1] = new Vector2(month, countIncome() - countExpense());

        for (int i = 0; i < items.Count; i++)
        {
            recalculateDemand(items[i],demandChangeDeviation,demandCalcMethod.randomNormalDistribution);
            //print($"New demand for {items[i].name} is {items[i].bought_number}");
        }
    }

    void Start()
    {
        addDebug();
    }

    [ContextMenu("Add debug item")]
    public void addDebug()
    {
        items.Add(new Item());
        items.Last().name = "яблоко";
        items.Last().buying_price = 5;
        items.Last().bought_number = 5;
        items.Last().selling_price_min = 10;
        items.Last().sold_number = 5;
    }
    public float countIncome()
    {
        float res = 0;

        foreach (var item in items)
        {
            if (item.sold_number <= item.bought_number)
            {
                res += item.selling_price_min * item.sold_number * adModifier;
            }
            else
            {
                res += item.selling_price_min * item.bought_number * adModifier;
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
        res += rent;
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
        Item item = new Item("Багет",new float[]{ 40, 80, 120, 200, 500 });
        item.selling_price = 100;
        float yMean = (item.demand_max + item.demand_min) / 2f;
        float xMean = (item.selling_price_max + item.selling_price_min) / 2f;
        float m = (((item.selling_price_max - xMean) * (item.demand_min - yMean)) + ((item.selling_price_min - xMean) * (item.demand_max - yMean))) /
            (MathF.Pow(item.selling_price_max - xMean, 2f) + MathF.Pow(item.selling_price_min - xMean, 2f));
        float b = yMean - m * xMean;
        item.sold_number = m * item.selling_price + b;
        print(item.sold_number);
    }
}