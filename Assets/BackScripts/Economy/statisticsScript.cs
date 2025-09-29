using UnityEngine;

public class statisticsScript : MonoBehaviour, ITickable, ISaveLoadable
{
    public statistics statistics;
    [Header("SaveLoadManager")]
    public saveLoadManager saveLoadManager;

    public void load(saveData data)
    {
        this.statistics = data.statistics;
    }
    public void save(ref saveData data)
    {
        data.statistics = this.statistics;
    }


    public void nextTurn(int month, int year)
    {
        if(month == 1 && year!=1)
        {
            saveLoadManager.saveStatistics2NewFile(statistics, year-1);
        }
    }

    [ContextMenu("Dump everything to file")]
    public void dump()
    {
        saveLoadManager.saveStatistics2NewFile(statistics,0);
    }
}
