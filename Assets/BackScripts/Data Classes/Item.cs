using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
