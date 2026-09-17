using UnityEngine;
using UnityEngine.SceneManagement;

public class clickToStore : MonoBehaviour
{
    public string storeSceneName;
    public saveLoadManager saveLoadManager;

    [SerializeField] private float dragTolerancePixels = 5f;

    private Vector3 pressScreenPos;
    private bool isPressing = false;

    private void OnMouseDown()
    {
        isPressing = true;
        pressScreenPos = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (!isPressing) return;
        isPressing = false;

        if (Vector3.Distance(pressScreenPos, Input.mousePosition) <= dragTolerancePixels)
            OnClick();
    }

    private void OnClick() 
    {
        var saveData = saveLoadManager.loadData();
        saveData.targetCanvas = 4; //добавить индекс для экрана с магазином
        saveLoadManager.saveData(saveData);

        SceneManager.LoadScene(storeSceneName);
    }
}
