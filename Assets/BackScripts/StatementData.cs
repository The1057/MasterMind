using UnityEngine;



[System.Serializable]
public class StatementData
{
    public string statement;        // Утверждение
    public bool isTrue;             // Правда или нет
    public int correctZoneId;
    [TextArea] public string explanation;
}