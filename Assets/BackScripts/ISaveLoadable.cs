using UnityEngine;

public interface ISaveLoadable
{
    public abstract void save(ref saveData data);
    public abstract void load(saveData data);
}
