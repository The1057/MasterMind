public enum LegalForm
{
    OOO, 
    IP 
}

[System.Serializable]
public class StatementData
{
    public string statement;        // Утверждение
    public bool isTrue;             // Правда или нет
    public LegalForm correctForm;
    public string explanation;      // Пояснение
}