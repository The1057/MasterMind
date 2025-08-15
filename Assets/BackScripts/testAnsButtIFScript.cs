using UnityEngine;

public class testAnsButtIFScript : MonoBehaviour
{
    public TestManager2 testManager;

    public void CheckAnswer()
    {
        testManager.IFCheckAnswer();
    }

    // Эти методы для совместимости с другими типами вопросов
    public void correct()
    {
        if (testManager != null)
            testManager.MCcorrectOption();
    }

    public void incorrect()
    {
        if (testManager != null)
            testManager.MCIncorrectOption();
    }

    public void accept()
    {
        if (testManager != null)
            testManager.MCAccept();
    }
}