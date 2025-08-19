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
    public saveData()
    {

    }
}


[System.Serializable]
public class storeData
{
    public string name = "";

    public List<Item> items = new List<Item>();
    public float adModifier = 1.1f;

    public List<Vector2> points4Graph = new List<Vector2>();
    public List<Vector2> points4GraphFinal = new List<Vector2>();
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
    public string name = "";
    public float selling_price = 0;
    public float buying_price = 0;
    public float sold_number = 0;
    public float bought_number = 0;
    //0 - цена покупки
    //1 - цена продажи
    //2 - число закупок
    //3 - число продаж
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
    public biz_niche first_niche;
    public legal_form legal_form;
    public tax_system tax_system;
}