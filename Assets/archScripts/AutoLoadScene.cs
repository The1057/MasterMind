using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadScene : MonoBehaviour
{
    public string sceneName;
    private void OnEnable()
    {
        SceneManager.LoadScene(sceneName,LoadSceneMode.Single);
    }
    private void Awake()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
