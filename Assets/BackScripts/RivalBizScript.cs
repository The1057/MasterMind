using System.Linq;
using UnityEngine;

public enum rivalClass
{
    weak = -1,
    normal = 0,
    strong = 1,
}
public class RivalBizScript : ISaveLoadable, ITickable
{
    public rivalBizData rivalBizData = new();

    public const float weakBizMod = 0.9f;
    public const float normalBizMod = 1f;
    public const float strongBizMod = 1.1f;

    public void save(ref saveData data)
    {
        if (data.StoreDatas.Count <= rivalBizData.storeID)
        {
            data.StoreDatas.Add(new storeData());
        }
        data.RivalDatas[rivalBizData.storeID].items = rivalBizData.items;
        data.RivalDatas[rivalBizData.storeID].points4Graph = rivalBizData.points4Graph;
        data.RivalDatas[rivalBizData.storeID].points4GraphFinal = rivalBizData.points4GraphFinal;
        data.RivalDatas[rivalBizData.storeID].storeID = rivalBizData.storeID;
        data.RivalDatas[rivalBizData.storeID].name = rivalBizData.name;
        data.RivalDatas[rivalBizData.storeID].Class = rivalBizData.Class;
    }
    public void load(saveData data)
    {
        rivalBizData.points4Graph = data.RivalDatas[rivalBizData.storeID].points4Graph;
        rivalBizData.points4GraphFinal = data.RivalDatas[rivalBizData.storeID].points4GraphFinal;
        rivalBizData.name = data.RivalDatas[rivalBizData.storeID].name;
        rivalBizData.Class = data.RivalDatas[rivalBizData.storeID].Class;
    }

    public void nextTurn(int month, int year)
    {

    }

    [ContextMenu("Add debug item")]
    public void addDebug()
    {
        rivalBizData.items.Add(new Item());
        rivalBizData.items.Last().name = "€блоко";
        rivalBizData.items.Last().buying_price = 5;
        rivalBizData.items.Last().bought_number = 5;
        rivalBizData.items.Last().selling_price = 10;
        rivalBizData.items.Last().sold_number = 5;
    }

    public float countIncome()
    {
        float res = 0;

        foreach (var item in rivalBizData.items)
        {
            if (item.sold_number <= item.bought_number)
            {
                res += item.selling_price * item.sold_number;
            }
            else
            {
                res += item.selling_price * item.bought_number;
            }
        }

        return res;
    }
    public float countExpense()
    {
        float res = 0;

        foreach (var item in rivalBizData.items)
        {
            res += item.buying_price * item.bought_number;
        }

        return res;
    }

    public float countProfit()
    {
        float profit = countIncome() - countExpense();
        switch (rivalBizData.Class)
        {
            case rivalClass.weak:
                return profit * weakBizMod;
            break;

            case rivalClass.normal:
                return profit * normalBizMod;
            break;

            case rivalClass.strong:
                return profit * strongBizMod;
            break;

            default:
                return profit;
        }

    }
    public void recalculateDemand(ref float currentDemand, float deviation, storeScript.demandCalcMethod calcMethod)
    {
        System.Random random = new System.Random();
        switch (calcMethod)
        {
            case storeScript.demandCalcMethod.normalDistribution:

                double randomsSum = 0;
                for (int i = 0; i < 12; i++)
                {
                    randomsSum += random.NextDouble();
                }
                randomsSum -= 6;
                currentDemand = (float)randomsSum * deviation + currentDemand;

                break;
            case storeScript.demandCalcMethod.linearDistribution:

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

}
