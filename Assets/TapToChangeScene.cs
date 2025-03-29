using UnityEngine;
using UnityEngine.SceneManagement;

public class TapToChangeScene : MonoBehaviour
{
    public string nextSceneName = ""; // ��������� �����

    void Update()
    {
   
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            
            SceneManager.LoadScene(nextSceneName);
        }

    }
}
