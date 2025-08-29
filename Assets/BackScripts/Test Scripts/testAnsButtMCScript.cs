using UnityEngine;

public class testAnsButtMCScript : MonoBehaviour
{
    public TestManager2 testManager;
    public void correct()
    {
        testManager.MCcorrectOption();
    }
    public void incorrect()
    {
        testManager.MCIncorrectOption();
    }
    public void accept()
    {
        testManager.MCAccept();
    }
}
