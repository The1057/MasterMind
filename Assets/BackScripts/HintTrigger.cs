using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    public void ClickStartSeries(int id)
    {
        if (TutorManager.instance != null)
        {
            TutorManager.instance.StartSeries(id);
        }
    }
}