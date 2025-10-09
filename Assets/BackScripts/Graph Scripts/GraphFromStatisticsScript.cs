using UnityEngine;
using System.Collections.Generic;
using TMPro;

public enum stat
{    
    profit = 0,
    statTaxExpense = 1,
    constExpense = 2,
    ROS = 3,
    income = 4,
    expense = 5,
}
public class GraphFromStatisticsScript : MonoBehaviour
{
    public GameObject graphSetupPrefab;
    private graphSetup graphSetup;
    public saveLoadManager saveLoadManager;
    public TMP_Dropdown yearList;

    public int statisticsYear = 1;
    public stat statType = stat.profit;

    public Vector3 statisticsPosition = new Vector3(0, 0);

    private statistics currentStatistics = new statistics();
    public void loadStatByYear(int year)
    {
        if (graphSetup != null)
        {
            graphSetup.destroyGraph();
        }
        if (year < 1)
        {
            year = 1;
        }
        graphSetup = Instantiate(graphSetupPrefab).GetComponent<graphSetup>();
        currentStatistics = saveLoadManager.readStatisticsFromFile(statisticsYear);
        switch (statType) 
        {
            case stat.income:
                graphSetup.graphData.points = float2VectorList(currentStatistics.incomeStat);
                graphSetup.points = float2VectorList(currentStatistics.incomeStat);
                break;


            case stat.expense:
                graphSetup.graphData.points = float2VectorList(currentStatistics.expenceStat);
                graphSetup.points = float2VectorList(currentStatistics.expenceStat);
                break;


            case stat.profit:
            graphSetup.graphData.points = float2VectorList(currentStatistics.profitStat);
            graphSetup.points = float2VectorList(currentStatistics.profitStat);
            break;


            case stat.statTaxExpense:
                graphSetup.graphData.points = float2VectorList(currentStatistics.taxExpenseStat);
                graphSetup.points = float2VectorList(currentStatistics.taxExpenseStat);
            break;


            case stat.constExpense:
                graphSetup.graphData.points = float2VectorList(currentStatistics.constExpenseStat);
                graphSetup.points = float2VectorList(currentStatistics.constExpenseStat);
            break;


            case stat.ROS:
                graphSetup.graphData.points = float2VectorList(currentStatistics.ROSStat);
                graphSetup.points = float2VectorList(currentStatistics.ROSStat);
            break;

        }

        graphSetup.transform.SetParent(transform, false);
        graphSetup.transform.position = transform.position + statisticsPosition;
        graphSetup.setup();
    }

    public void setStatType(int type)
    {
        statType = (stat)type;
    }
    public void setYear(int index)
    {
        print($"Trying to set year to {index}");
        int year = yearList.options.Count - index;
        print($"Set year to {year}");
        statisticsYear = year;
    }
    private List<Vector2> float2VectorList(float[] floats)
    {
        List<Vector2> list = new List<Vector2>();
        for (int i = 0; i < floats.Length; i++)
        {
            list.Add(new Vector2((float)i, floats[i]));
        }
        return list;
    }
}
