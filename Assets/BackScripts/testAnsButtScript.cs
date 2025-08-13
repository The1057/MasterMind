using UnityEngine;

public class testAnsButtScript : MonoBehaviour
{
    public TestManager2 testManager;
    public void correct_oc()
    {
        testManager.correctOption();
    }
    public void incorrect()
    {
        testManager.incorrectOption();
    }
}
