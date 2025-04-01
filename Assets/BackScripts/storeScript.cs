using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class storeScript : MonoBehaviour
{
    public ClockScript clock;

    public Dictionary<string, float[]> items = new Dictionary<string, float[]> { };
    //0 - цена покупки
    //1 - цена продажи
    //2 - число закупок
    //3 - число продаж

    public float adModifier = 1.1f;

    int newID = 0;


    List<Vector2> points4Graph = Enumerable.Repeat(new Vector2(1, 0), 12).ToList();
    public List<Vector2> points4GraphFinal  = Enumerable.Repeat(new Vector2(0, 0), 12).ToList();
    //var l = Enumerable.Repeat(new Vector2(0,0),12).ToList();
    void Start()
    {
        clock = GameObject.FindGameObjectWithTag("ClockTag").GetComponent<ClockScript>();
        addDebug();
    }

    void Update()
    {
        if (clock.isNextTurn())
        {
            //Debug.Log($"доходы: {countIncome()}, расходы: {countExpense()}, прибыль: {countIncome()-countExpense()}");
            //print($"доходы: {countIncome()}, расходы: {countExpense()}, прибыль: {countIncome() - countExpense()}");
            print(points4Graph[0]);
            if(clock.getMonth() == 1)
            {
                points4GraphFinal = new List<Vector2>(points4Graph);
            }
            points4Graph[clock.getMonth() - 1] = new Vector2(clock.getMonth(), countIncome() - countExpense());
        }
    }

    [ContextMenu("Add debug item")]
    public void addDebug()
    {
        items.Add(newID.ToString(),new float[] { 10, 20, 5, 5 });
        newID++;
    }
    public float countIncome()
    {
        float res = 0;

        foreach (var item in items)
        {
            res += item.Value[1] * item.Value[3] * adModifier;
        }

        return res;
    }
    public float countExpense()
    {
        float res = 0;

        foreach (var item in items)
        {
            res += item.Value[0] * item.Value[2];
        }

        return res;
    }
}
