using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class saveData
{
    public List<storeData> StoreDatas = new List<storeData>();
    public moneyData MoneyData = new moneyData();
    public clockData ClockData = new clockData();
    
    public saveData()
    {

    }
}
