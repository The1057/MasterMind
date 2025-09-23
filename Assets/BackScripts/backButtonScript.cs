using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum backButtonMode
{
    lastScene = 0,
    theory = 1,
    test = 2
}
public class backButtonScript : MonoBehaviour
{

    public saveLoadManager saveLoadManager;
    public CanvasSwitcher1 canvasSwitcher;
    private bool isLoading = false;
    public backButtonMode mode = backButtonMode.lastScene;
    public GameObject theoryManager;
    public GameObject testManager;
    public List<int> testIndList = new List<int>();
    TestManager2 manager;
    CanvasSequenceManager23 tManager;
    [Space(10)]
    [Header("Canvas List")]
    public List<GameObject> lastCanvases;
    void Start()
    {
        if (mode == backButtonMode.test)
        {
            print("Trying to set testManager");
            manager = testManager.GetComponent<TestManager2>();
        }
        else if (mode == backButtonMode.theory)
        {
            tManager = theoryManager.GetComponent<CanvasSequenceManager23>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == backButtonMode.test)
        {
            if (testIndList.Count == 0 || manager.currentQuestion != testIndList.Last())
            {
                testIndList.Add(manager.currentQuestion);
            }
        }
        else if (mode == backButtonMode.lastScene)
        {
            if (lastCanvases.Count > 0)
            {
                if (lastCanvases.Last() != canvasSwitcher.currentActiveCanvas)
                {
                    lastCanvases.Add(canvasSwitcher.currentActiveCanvas);
                }
            }
            else 
            {
                lastCanvases.Add(canvasSwitcher.initialCanvas);
            }
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            switch (mode)
            {
                case backButtonMode.lastScene:                    
                    print("back");
                    loadPrevoiusCanvas();
                    break;


                case backButtonMode.theory:
                    tManager.ShowPreviousCanvas();
                    break;


                case backButtonMode.test:
                    if (testIndList.Count > 1)
                    {
                        print($"Trying to set question to index {testIndList[testIndList.Count - 2]}");
                        manager.setQuestionByIndex(testIndList[testIndList.Count - 2]);
                        testIndList.Remove(testIndList.Last());
                    }
                    break;


                default:

                    break;
            }
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

    public void loadPrevoiusCanvas()
    {
        if(lastCanvases.Count > 1)
        {
            lastCanvases.Remove(lastCanvases.Last());
            canvasSwitcher.SwitchToCanvas(lastCanvases.Last());
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
