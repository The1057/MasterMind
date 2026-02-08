using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.IO.Archive;

[RequireComponent(typeof(Button))]
public class SceneButtonLoader : MonoBehaviour
{
    [Header("Scene to Load")]
    [SerializeField] private string sceneName;

    [Header("Press Animation")]
    [SerializeField, Tooltip("Scale multiplier when button is pressed")] private float pressedScale = 0.95f;
    [SerializeField, Tooltip("Duration of scale animation")] private float scaleDuration = 0.033f; // ускорено 1.5x

    [Header("Canvas Loading")]
    public saveLoadManager saveLoadManager;

    private Button _button;
    private Vector3 _initialScale;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _initialScale = transform.localScale;

        _button.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        StartCoroutine(DoTransition());
    }

    private IEnumerator DoTransition()
    {
        // Быстрое сжатие кнопки
        yield return ScaleRoutine(_initialScale, _initialScale * pressedScale, scaleDuration);
        yield return ScaleRoutine(transform.localScale, _initialScale, scaleDuration);

        // Мгновенная загрузка сцены
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator ScaleRoutine(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.LerpUnclamped(from, to, elapsed / duration);
            yield return null;
        }
        transform.localScale = to;
    }

    public void setTargetCanvas(int canvasIndex)
    {
        var saveData = saveLoadManager.loadData();
        saveData.targetCanvas = canvasIndex;
        print(saveData.PlayerData.player_name);
        //switch (canvasIndex)
        //{
        //    case 0:
        //        saveData.targetCanvas = "Tasks";
        //    break;


        //    case 1:
        //        saveData.targetCanvas = "Shop";
        //    break;


        //    case 2:
        //        saveData.targetCanvas = "Archive";
        //    break;


        //    case 3:
        //        saveData.targetCanvas = "Profile";
        //    break;
        //}
        saveLoadManager.saveData(saveData);
        print(saveLoadManager.loadData().PlayerData.player_name);
    }
}
