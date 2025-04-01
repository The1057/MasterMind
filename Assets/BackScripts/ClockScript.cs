using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockScript : MonoBehaviour
{
    bool forceNextTurn;
    bool nextTurnFlag;
    bool turnPendingFlag;

    public bool turnCriteria;

    int month=1;
    int year=1;

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
