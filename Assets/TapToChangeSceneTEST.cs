using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TapToChangeScene : MonoBehaviour
{
    public string nextSceneName = ""; // Название следующей сцены
    public Button changeSceneButton; // Кнопка для смены сцены

    void Start()
    {
        if (changeSceneButton != null)
        {
            changeSceneButton.onClick.AddListener(ChangeScene);
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("MAP");
        SceneManager.LoadScene("TASK");
        SceneManager.LoadScene("CHOOSEBUSINES");
    }

  

}
