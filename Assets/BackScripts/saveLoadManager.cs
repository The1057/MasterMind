using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public class saveLoadManager : MonoBehaviour
{
    public static saveLoadManager instance { get; private set; }
    private saveData SaveData;
    private List<ISaveLoadable> saveLoadableObjects;

    public string saveDirPath = "";
    public string saveFileName = "save.json";
    public GameObject storeObject;
    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Only one saveLoadManager can exist at a time");
        }
        instance = this;
    }
    public void Start()
    {
        SaveData = new saveData();
        saveLoadableObjects = findAllSaveLoadables();
        saveDirPath = Application.persistentDataPath;
    }
    [ContextMenu("Save Game")]
    public void saveGame()
    {
        saveLoadableObjects = findAllSaveLoadables();
        foreach(var obj in saveLoadableObjects)//getting all the data from everywhere
        {
            obj.save(ref SaveData);
        }
        string fullPath = Path.Combine(saveDirPath, saveFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //creating directory 

            string rawJSON = JsonUtility.ToJson(SaveData,true);
            //serializing

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(rawJSON);//magic to write to file
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"Error while saving data from file: {fullPath} \n {e}");
        }
    }
    [ContextMenu("Load Game")]
    public void loadGame()
    {
        string fullPath = Path.Combine(saveDirPath, saveFileName);
        if (File.Exists(fullPath))
        {
            try
            {
                string rawJSON;
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        rawJSON = reader.ReadToEnd();//magic to read from file
                    }
                }
                SaveData = JsonUtility.FromJson<saveData>(rawJSON);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading data from file: {fullPath} \n {e}");
            }

            if (SaveData == null)
            {
                Debug.LogError("No data found. Creating new instance of saveData");
                SaveData = new saveData();
            }


            deleteAllStoresOnScene();//удаляем существующие торговые точки
            foreach (var store in SaveData.StoreDatas)//создаём все торговые точки
            {
                Instantiate(storeObject).GetComponent<storeScript>().storeId = store.storeID;
            }
            saveLoadableObjects = findAllSaveLoadables();
            foreach (var obj in saveLoadableObjects)
            {
                obj.load(SaveData);
            }
        }
    }

    [ContextMenu("Destroy Stores")]
    private void deleteAllStoresOnScene()
    {
        var stores = GameObject.FindGameObjectsWithTag("storeTag");
        foreach(var store in stores)
        {
            Destroy(store);
        }
    }
    private List<ISaveLoadable> findAllSaveLoadables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).
            OfType<ISaveLoadable>().ToList();
    }
}
