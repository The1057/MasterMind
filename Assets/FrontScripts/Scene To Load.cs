using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneToLoad : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    public void SwitchScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
     
    }
}
