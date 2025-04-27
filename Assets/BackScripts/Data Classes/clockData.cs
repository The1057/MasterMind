using UnityEngine;


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
