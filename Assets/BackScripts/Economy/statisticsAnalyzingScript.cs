using UnityEngine;

public class statisticsAnalyzingScript : MonoBehaviour, ITickable
{
    public statisticsScript statisticsScript;
    [Header("Достижения для статистики")]
    public float desiredROS = 85f;
    public float desiredYearProfit = 100000;
    public void nextTurn(int month, int year)
    {
        if (month == 1)
        {
            if (checkROS())
            {
                print($"Рентабельность продаж выше нужной ({yearAVG(statisticsScript.statistics.ROSStat)}), молодец, возьми с полки пирожок");
            }
            else
            {
                print($"Рентабельность продаж ниже нужной({yearAVG(statisticsScript.statistics.ROSStat)}), смерть в нищите");
            }
            if (checkProfit())
            {
                print($"Прибыль продаж выше нужной ({yearSum(statisticsScript.statistics.profitStat)}), молодец, возьми с полки пирожок");
            }
            else
            {
                print($"Прибыль продаж ниже нужной({yearSum(statisticsScript.statistics.profitStat)}), смерть в нищите");
            }
        }
    }
    bool checkROS()
    {
        return yearAVG(statisticsScript.statistics.ROSStat) >= desiredROS;
    }
    bool checkProfit()
    {
        return yearSum(statisticsScript.statistics.profitStat) >= desiredYearProfit;
    }
    float yearAVG(float[] stat)
    {
        float avg = 0;

        foreach (var a in stat)
        {
            avg += a;
        }

        return avg/12;
    }
    float yearSum(float[] stat)
    {
        float sum = 0;

        foreach(var a in stat)
        {
            sum += a;
        }

        return sum;
    }
}
