using UnityEngine;

public class testAnsButtScript : MonoBehaviour
{
    public TestManager2 testManager;
    public void correct()
    {
        testManager.correctOption();
    }
    public void incorrect()
    {
        testManager.incorrectOption();
    }
}
