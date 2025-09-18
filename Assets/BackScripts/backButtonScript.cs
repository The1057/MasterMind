using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class backButtonScript : MonoBehaviour
{

    public saveLoadManager saveLoadManager;
    private bool isLoading = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            print("back");
            loadPreviousScene();
        }
    }

    private void OnDisable()
    {
        if (!isLoading)
        {
            var saveData = saveLoadManager.loadData();
            saveData.lastScenes.Add(SceneManager.GetActiveScene().name);
            saveLoadManager.saveData(saveData);
        }
    }

    public void loadPreviousScene()
    {
        var saveData = saveLoadManager.loadData();
        if (saveData.lastScenes != null && saveData.lastScenes.Count > 0)
        {
            var sceneName = saveData.lastScenes.Last();
            saveData.lastScenes.Remove(saveData.lastScenes.Last());
            saveLoadManager.saveData(saveData);
            SceneManager.LoadScene(sceneName);
            isLoading = true;
        }
    }

    [ContextMenu("Set last scene to Shop")]
    public void debug()
    {
        var saveData = saveLoadManager.loadData();        
        saveData.lastScenes.Add("Shop");
        print(saveData.lastScenes);
        saveLoadManager.saveData(saveData);
        
    }
    [ContextMenu("Clean last scene list")]
    public void cleanSceneList()
    {
        var saveData = saveLoadManager.loadData();
        saveData.lastScenes.Clear();
        saveLoadManager.saveData(saveData);
        print("List has been cleared");
    }
}
