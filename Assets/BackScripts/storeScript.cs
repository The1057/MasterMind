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
        normalDistribution = 0,
        linearDistribution = 1
    }


    public int storeId = 0;

    public List<Item> items = new List<Item>();
    
    public float adModifier = 1.1f;
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
            recalculateDemand(ref items[i].sold_number,demandChangeDeviation,demandCalcMethod.normalDistribution);
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
        items.Last().name = "€блоко";
        items.Last().buying_price = 5;
        items.Last().bought_number = 5;
        items.Last().selling_price = 10;
        items.Last().sold_number = 5;
    }
    public float countIncome()
    {
        float res = 0;

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

        return res;
    }
    public void recalculateDemand(ref float currentDemand,float deviation, demandCalcMethod calcMethod)
    {
        System.Random random = new System.Random();
        switch (calcMethod)
        {
            case demandCalcMethod.normalDistribution:

                double randomsSum = 0;
                for(int i = 0; i < 12; i++)
                {
                    randomsSum += random.NextDouble();
                }
                randomsSum -= 6;
                currentDemand = (float)randomsSum * deviation + currentDemand;
                
                break;
            case demandCalcMethod.linearDistribution:

                currentDemand = (currentDemand - deviation) * (float)random.NextDouble() + currentDemand;

                break;
            default:

                break;

        }
        if (currentDemand < 0)
        {
            currentDemand = 0;
        }
    }

    [ContextMenu("Print first item")]
    public void showDebug()
    {
        print($"id:{storeId}\n name: {items[0].name} " +
            $"\n BP: {items[0].buying_price} " +
            $"\n SP: {items[0].selling_price}");
    }
}
