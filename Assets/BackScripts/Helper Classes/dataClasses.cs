using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public enum biz_niche
{
    breadShop = 0,
    bookShop = 1,
    clothShop = 2
}
[Serializable]
public enum legal_form
{
    OOO = 0,
    IP = 1,
}
[Serializable]
public enum tax_system
{
    USN = 0,
    PSN = 1,
}



[System.Serializable]
public class saveData
{
    public List<storeData> StoreDatas = new List<storeData>();
    public List<rivalBizData> RivalDatas = new List<rivalBizData>();
    public moneyData MoneyData = new moneyData();
    public clockData ClockData = new clockData();
    public playerData PlayerData = new playerData();
    public statistics statistics = new statistics();
    public adSystemData adSystemData = new adSystemData();
    public List<string> lastScenes = new();
    public int targetCanvas = 0;
    public List<TaskSaveData> TasksData = new List<TaskSaveData>();
    public List<float> TheoryCanvasAlphas = new List<float>();
    public List<PlateSaveInfo> ArchiveProgress = new List<PlateSaveInfo>();
    public saveData()
    {

    }
}

[System.Serializable]
public class adSystemData
{
    public float globalAdModifier = 1;
    public AdUpgradeTree internetAd = new();
    public activeTree activeTree;
}

[System.Serializable]
public class storeData
{
    public string name = "";

    public List<Item> items = new List<Item>();
    public float adModifier = 1.1f;
    public float constExpense = 1000;//постоянные затраты: аренда, зарплата
    public float randomExpenseMin = 0.03f;
    public float randomExpenseMax = 0.05f;
    public float demandChangeDeviation = 5;

    public int storeID = 0;
}


[System.Serializable]
public class moneyData
{
    public float player_money = 0;
    public int player_gems = 0;

    public List<storeScript> storeList;
    public float totalIncome = 0, totalExpense = 0;

    public float profit;
    public float operationProfit;

    public float tax1 = 1;
    public float tax2 = 0.87f;
}


[System.Serializable]
public class graphData
{
    public Vector2Int gridSize = new Vector2Int(12, 12);
    public List<Vector2> points = new List<Vector2>();
    public float gridThickness = 4f;
    public float lineThickness = 6f;
    public Color lineColor = new Color(1, 1, 1);
    public Color zoneBottomColor = new Color(0x3B / 256f, 0x07 / 256f, 0x5C / 256f);
    public Color zoneTopColor = new Color(0x90 / 256f, 0x09 / 256f, 0x1B / 256f);
    public Color gridColor = new Color(0.8f, 0.8f, 0.8f);
    public int graphHeight = 500;

    public void saveToGraphFile(string graphDataFileName = "testGraph.json")
    {
        var graphDataDirPath = Application.persistentDataPath;

        // \/ saving example graph to file \/
        string fullPath = Path.Combine(graphDataDirPath, graphDataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //creating directory 

            string rawJSON = JsonUtility.ToJson(this, true);
            //serializing

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(rawJSON);//magic to write to file
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while saving data from file: {fullPath} \n {e}");
        }
        // /\ saving example graph to file /\
    }
}


[System.Serializable]
public class clockData
{
    public bool forceNextTurn;
    public bool nextTurnFlag;
    public bool turnPendingFlag;

    public bool turnCriteria;

    public int month = 1;
    public int year = 1;
}


[System.Serializable]
public class Item
{
    public int id = 0;
    public string name = "";
    public float selling_price_min = 0;
    public float selling_price_max = 0;
    public float selling_price = 0;
    public float buying_price = 0;
    public float sold_number = 0;
    public float bought_number = 0;
    public float demand_min = 0;
    public float demand_max = 0;
    //0 - цена покупки
    //1 - цена продажи
    //2 - число закупок
    //3 - число продаж
    public Item(int id, string name, float[] args)
    {
        this.id = id;
        this.name = name;
        buying_price = args[0];
        selling_price_min = args[1];
        selling_price_max = args[2];
        demand_min = args[3];
        demand_max = args[4];
    }
    public Item()
    {

    }
}

[System.Serializable]
public class TaskSaveData
{
    public int taskIndex;
    public List<SubtaskSaveData> subtasksData = new List<SubtaskSaveData>();
    public bool isExpanded;
    public bool isCompleted; 
}

[System.Serializable]
public class SubtaskSaveData
{
    public int subtaskIndex;
    public bool isCompleted;
}

[System.Serializable]
public class rivalBizData
{
    public string name = "";

    public List<Item> items = new List<Item>();
    public rivalClass Class = 0;

    public List<Vector2> points4Graph = new List<Vector2>();
    public List<Vector2> points4GraphFinal = new List<Vector2>();
    public int storeID = 0;
}

[System.Serializable]
public class playerData
{
    public string player_name = "";
    public string player_gender = "";
    public int storePicIndex = 0;
    public biz_niche first_niche;
    public legal_form legal_form;
    public tax_system tax_system;
}

[System.Serializable]
public class statistics
{
    public float[] incomeStat = new float[12];
    public float[] expenceStat = new float[12];
    public float[] profitStat = new float[12];
    public float[] taxExpenseStat = new float[12];
    public float[] constExpenseStat = new float[12];
    public float[] ROSStat = new float[12];
}