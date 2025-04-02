using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class TapToProfile : MonoBehaviour
{
    public string maleProfileSceneName = "ProfileMan";
    public string femaleProfileSceneName = "ProfileW";
    private string genderFilePath;
    private string playerGender = "";
    private bool genderLoaded = false;

    void Start()
    {
#if UNITY_EDITOR
        string scriptPath = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.MonoScript.FromMonoBehaviour(this));
        string scriptDirectory = Path.GetDirectoryName(scriptPath);
        genderFilePath = Path.Combine(scriptDirectory, "player_gender.txt");
#else
        genderFilePath = Path.Combine(Application.persistentDataPath, "player_gender.txt");
#endif

        LoadGender();
    }

    void Update()
    {
        // Проверяем тап на экран только после того, как гендер был загружен
        if (genderLoaded && (Input.GetMouseButtonDown(0) || Input.touchCount > 0))
        {
            LoadProfileScene();
        }
    }

    void LoadGender()
    {
        if (File.Exists(genderFilePath))
        {
            try
            {
                playerGender = File.ReadAllText(genderFilePath).Trim().ToUpper();
                Debug.Log("Загруженный гендер: " + playerGender);
                genderLoaded = true; // Устанавливаем флаг, что гендер загружен
            }
            catch (System.Exception e)
            {
                Debug.LogError("Ошибка при чтении файла гендера: " + e.Message);
            }
        }
        else
        {
            Debug.Log("Файл гендера не найден. Загрузка будет отложена.");
            // Не устанавливаем genderLoaded в true, чтобы предотвратить загрузку сцены без гендера
        }
    }

    void LoadProfileScene()
    {
        if (playerGender == "М")
        {
            SceneManager.LoadScene(maleProfileSceneName);
        }
        else if (playerGender == "Ж")
        {
            SceneManager.LoadScene(femaleProfileSceneName);
        }
        else if (genderLoaded) // Проверяем, что попытка загрузки была после чтения файла
        {
            Debug.LogWarning("Неизвестное значение гендера: " + playerGender + ". Переход отменен.");
            // Не загружаем никакую сцену, если значение не "М" и не "Ж"
        }
        else
        {
            Debug.Log("Гендер еще не загружен. Действие отложено.");
        }
    }
}