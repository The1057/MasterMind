using UnityEngine;

public class playerDataClass : MonoBehaviour, ISaveLoadable
{
    public playerData data;

    public void load(saveData data)
    {
        this.data = data.PlayerData;
    }

    public void save(ref saveData data)
    {
        data.PlayerData = this.data;
    }
}
