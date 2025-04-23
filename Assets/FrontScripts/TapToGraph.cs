
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TapToGraph : MonoBehaviour
{
    // ����������� ������ � ����������
    [SerializeField] private Button transitionButton;

    void Start()
    {
        // ���������, ��� ������ ���������
        if (transitionButton != null)
        {
            // ��������� ��������� ������� �� ������� ������
            transitionButton.onClick.AddListener(GoToAnalizzzScene);
        }
        else
        {
            Debug.LogError("������ �� ��������� � ����������!");
        }
    }

    // ����� ��� �������� �� ����� Analizzz
    void GoToAnalizzzScene()
    {
        SceneManager.LoadScene("Analizzz");
    }

    // ������� ��������� ��� ����������� �������
    void OnDestroy()
    {
        if (transitionButton != null)
        {
            transitionButton.onClick.RemoveListener(GoToAnalizzzScene);
        }
    }
}