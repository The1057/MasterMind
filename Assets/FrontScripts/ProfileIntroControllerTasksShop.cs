using UnityEngine;
using System.Collections;

/// <summary>
/// ������������ ����������� ����� ���������������� �������� ��� ������ �������� ������ �������.
/// </summary>
[DisallowMultipleComponent]
public class ProfileIntroControllerTasksShop : MonoBehaviour
{
    [Header("Intro Objects")]
    [Tooltip("������ GameObject'�� �� �����, ������� ��������������� ������������ ��� ������ �������� ������ �������")]
    public GameObject[] introObjects;

    private int _currentIndex;
    private bool _isAnimating;

    // �������� ������ ���������: 
    // true  � ���� ��� �� ��� ����� ���������� (����������� � PlayerPrefs);
    // false � ���� ��� �� ������ (��� ����������� ���������� ����� ���������).
    [SerializeField]
    private bool persistent = true;

    private const string PREF_KEY = "ProfileIntroPlayed";
    // ��� ����������� ���������� ���� �� ����� ������
    private static bool _sessionPlayed = false;

    void OnEnable()
    {
        bool hasPlayed = persistent
            ? (PlayerPrefs.GetInt(PREF_KEY, 0) == 1)
            : _sessionPlayed;

        if (!hasPlayed && introObjects != null && introObjects.Length > 0)
        {
            // �������� ��������
            _isAnimating = true;
            _currentIndex = 0;
            // ������������ ���, ���������� ������ ������
            for (int i = 0; i < introObjects.Length; i++)
                introObjects[i].SetActive(i == 0);
        }
        else
        {
            // ��� ���������� � ������ �������� �� � ��������� ���� ���������
            _isAnimating = false;
            if (introObjects != null)
                foreach (var obj in introObjects)
                    obj.SetActive(false);

            // ������ �� �����
            enabled = false;
        }
    }

    void Update()
    {
        if (!_isAnimating || introObjects == null || introObjects.Length == 0)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // �������� ������� � ��������� � ����������
            introObjects[_currentIndex].SetActive(false);
            _currentIndex++;

            if (_currentIndex < introObjects.Length)
            {
                introObjects[_currentIndex].SetActive(true);
            }
            else
            {
                // ����� �������� � ��������� ���� � ��������� ���������
                _isAnimating = false;

                if (persistent)
                {
                    PlayerPrefs.SetInt(PREF_KEY, 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    _sessionPlayed = true;
                }

                enabled = false;
            }
        }
    }
}
