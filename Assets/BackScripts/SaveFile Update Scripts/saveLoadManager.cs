using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public class saveLoadManager : MonoBehaviour, ITickable
{
    public static saveLoadManager instance { get; private set; }
    private saveData SaveData;
    private List<ISaveLoadable> saveLoadableObjects;

    public string saveDirPath = "";
    public string saveFileName = "save.json";
    public string statisticsDirName = "stat";
    public GameObject storeObject;
    public GameObject rivalObject;

    [Header("Canvas Switching")]
    public CanvasSwitcher1 CanvasSwitcher;
    public List<GameObject> coreCanvases;

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
        saveLoadableObjects = findAllSaveLoadables();
        saveDirPath = Application.persistentDataPath;

        loadGame();
        //print($"<color=red>Loaded name: {SaveData.PlayerData.player_name}");
        //SaveData = loadData();
        if (SaveData != null && CanvasSwitcher != null)
        {
            bool hasProfile = !string.IsNullOrEmpty(SaveData.PlayerData.player_name) &&
                              !string.IsNullOrEmpty(SaveData.PlayerData.player_gender);

            if (hasProfile)
            {
                CanvasSwitcher.SwitchToCanvas(coreCanvases[SaveData.targetCanvas]);
                Debug.Log($"Профиль есть, загружаем канвас: {coreCanvases[SaveData.targetCanvas].name}");
            }
            else
            {
                Debug.Log("Профиль не заполнен, переключение из saveLoadManager отменено.");
            }
        }
    }
    [ContextMenu("Save Game")]
    public void saveGame()
    {
        Debug.Log("Starting saving...");
        Debug.Log("Looking for saveable objects...");
        saveLoadableObjects = findAllSaveLoadables();
        Debug.Log("Getting data from saveable objects...");
        foreach (var obj in saveLoadableObjects)//getting all the data from everywhere
        {
            obj.save(ref SaveData);
        }
        Debug.LogWarning($"Saving! Turn flag: {SaveData.ClockData.turnCriteria}");
        string fullPath = Path.Combine(saveDirPath, saveFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //creating directory 

            string rawJSON = JsonUtility.ToJson(SaveData,true);
            //serializing
            Debug.Log("Writing data to file...");
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
        Debug.Log("Done saving");
    }
    [ContextMenu("Load Game")]
    public void loadGame()
    {
        Debug.Log("Starting loading....");
        string fullPath = Path.Combine(saveDirPath, saveFileName);

        // Инициализируем данные ПЕРЕД загрузкой, чтобы они не были null
        if (SaveData == null) SaveData = new saveData();

        if (File.Exists(fullPath))
        {
            try
            {
                string rawJSON;
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        rawJSON = reader.ReadToEnd();
                    }
                }
                // Десериализуем в существующий объект или создаем новый
                var loadedData = JsonUtility.FromJson<saveData>(rawJSON);
                if (loadedData != null) SaveData = loadedData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading: {e}");
            }
        }
        else
        {
            Debug.LogWarning("Save file not found. Using default data.");
        }

        Debug.LogWarning($"Loading! Turn flag: {SaveData.ClockData.turnCriteria}");

        Debug.Log("Deleting excess stores and rivals...");
        if (SaveData.StoreDatas != null && SaveData.StoreDatas.Count > 0)
        {
            Debug.Log("Deleting excess stores and rivals...");
            deleteAllStoresOnScene();
            // ... создание магазинов из SaveData ...
        }
        Debug.Log("Creating stores and rivals from save data...");
            foreach (var store in SaveData.StoreDatas)//создаём все торговые точки
            {
                Instantiate(storeObject).GetComponent<storeScript>().storeId = store.storeID;
            }
            foreach (var rival in SaveData.RivalDatas)//создаём всех соперников
            {
                Instantiate(rivalObject).GetComponent<RivalBizScript>().rivalBizData.storeID = rival.storeID;
            }
            Debug.Log("Looking for new saveable objects...");
            saveLoadableObjects = findAllSaveLoadables();
            Debug.Log("Loading save data to objects...");
            foreach (var obj in saveLoadableObjects)
            {
                obj.load(SaveData);
            }
        Debug.Log("Done loading");
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        // На мобилках это самый надежный способ. 
        // Если pauseStatus == true, значит игрок свернул игру или вышел на рабочий стол.
        if (pauseStatus)
        {
            saveGame();
            Debug.Log("Игра сохранена при уходе в паузу (Mobile)");
        }
    }

    //just load and save without placing shit
    public saveData loadData()
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
        }

        return SaveData;
        }
    public void saveData(saveData SaveData)
    {
        
        string fullPath = Path.Combine(saveDirPath, saveFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //creating directory 

            string rawJSON = JsonUtility.ToJson(SaveData, true);
            //serializing

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(rawJSON);//magic to write to file
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while saving data from file: {fullPath} \n {e}");
        }
    }
    public void saveStatistics2NewFile(statistics statistics, int year)
    {
        string fullPath = Path.Combine(saveDirPath, statisticsDirName + year.ToString() + ".json");
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            //creating directory 

            string rawJSON = JsonUtility.ToJson(statistics, true);
            //serializing

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(rawJSON);//magic to write to file
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error while saving data from file: {fullPath} \n {e}");
        }

    }
    public statistics readStatisticsFromFile(int year)
    {
        string fullPath = Path.Combine(saveDirPath, statisticsDirName + year.ToString() + ".json");
        statistics resStat = null;
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
                resStat = JsonUtility.FromJson<statistics>(rawJSON);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading data from file: {fullPath} \n {e}");
            }

            if (SaveData == null)
            {
                Debug.LogError("No data found. Creating new instance of statistics");
                resStat = new statistics();
            }
        }
        return resStat;
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
    [ContextMenu("Destroy Rivals")]
    private void deleteAllRivalsOnScene()
    {
        var rivals = GameObject.FindGameObjectsWithTag("rivalTag");
        foreach (var rival in rivals)
        {
            Destroy(rival);
        }
    }
    private List<ISaveLoadable> findAllSaveLoadables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).
            OfType<ISaveLoadable>().ToList();
    }

    public void nextTurn(int month, int year)
    {
        if (month == 1)
        {
            Debug.Log("Autosaving");
            saveGame();
        }
    }
    public void OnDisable()
    {
        //saveGame();
    }
    public void OnApplicationQuit()
    {
        //saveGame();
    }
    public void ResetAllProgress()
    {
        
        string mainSavePath = Path.Combine(saveDirPath, saveFileName);

        if (File.Exists(mainSavePath))
        {
            File.Delete(mainSavePath);
            Debug.Log("Основной файл сохранения удален.");
        }

        string[] statFiles = Directory.GetFiles(Application.persistentDataPath, statisticsDirName + "*.json");
        foreach (string file in statFiles)
        {
            File.Delete(file);
            Debug.Log($"Файл статистики удален: {file}");
        }

        SaveData = new saveData();

        SceneManager.LoadScene("RestartScene");

        Debug.Log("Прогресс полностью сброшен!");
    }
}
