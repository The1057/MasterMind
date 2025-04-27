using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockScript : MonoBehaviour, ISaveLoadable
{
    bool forceNextTurn;
    bool nextTurnFlag;
    bool turnPendingFlag;

    public bool turnCriteria;

    public int month=1;
    public int year=1;

    // Update is called once per frame
    void Update()
    {        
        if((turnCriteria && turnPendingFlag) || forceNextTurn)
        {
            StartCoroutine(inBetweenTurns());
            forceNextTurn = false;
            nextTurnFlag = true;
            turnPendingFlag = false;
            month++;
        } else if (turnPendingFlag)
        {
            turnPendingFlag = false;
        }
        if(month == 13)
        {
            year++;
            month = 1;
        }
    }

    public void save(ref saveData saveData)
    {
        saveData.ClockData.month = month;
        saveData.ClockData.year = year;
        saveData.ClockData.turnPendingFlag = turnPendingFlag;
        saveData.ClockData.turnCriteria = turnCriteria;
        saveData.ClockData.forceNextTurn = forceNextTurn;
        saveData.ClockData.nextTurnFlag = nextTurnFlag;
    }
    public void load(saveData loadData)
    {
        
        this.month = loadData.ClockData.month;
        this.year = loadData.ClockData.year;
        this.turnPendingFlag = loadData.ClockData.turnPendingFlag;
        this.turnCriteria = loadData.ClockData.turnCriteria;
        this.forceNextTurn = loadData.ClockData.forceNextTurn;
        this.nextTurnFlag = loadData.ClockData.nextTurnFlag;
    }
    IEnumerator inBetweenTurns()
    {
        yield return 0;
        nextTurnFlag = false;
    }
    public bool isNextTurn()
    {
        return nextTurnFlag;
    }
    [ContextMenu("Try next turn")]
    public void tryNextTurn() { turnPendingFlag = true; }

    [ContextMenu("Force next turn")]
    public void forceTurn() { forceNextTurn = true; }
    public int getMonth() { return month; } 
    public int getYear() { return year; } 
}
