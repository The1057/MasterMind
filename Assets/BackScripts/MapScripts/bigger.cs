using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bigger : MonoBehaviour
{
    [SerializeField] private Transform original;
    [SerializeField] private GameObject butEx;

    private bool isMoving = false;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float duration = 0.3f;
    private float elapsed = 0f;

    private GameObject ch;
    private Vector3 buttonPos;          //  ����������, ������ ��������
    private Vector2 startSize;
    private Vector2 targetSize;
    private Vector2 buttonSize;         //  �������� ������ ������
    private int ind;
    private Transform buts;

    private void Update()
    {
        if (!isMoving || ch == null) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // �������
        ch.transform.position = Vector3.Lerp(startPos, targetPos, t);

        var rt = ch.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = Vector2.Lerp(startSize, targetSize, t);

        if (t >= 1f)
        {
            isMoving = false;
            ch.transform.position = targetPos;
            
            if (rt != null)
                rt.sizeDelta = targetSize;

            // ���������� ������ ����� ��������� ������� � ������
            if (Vector3.Distance(targetPos, buttonPos) < 0.1f)
            {
                Destroy(ch);
                ch = null;
            }
            else {
                buts = transform.Find($"" + ind);
                buts.SetAsLastSibling(); 
                buts.gameObject.SetActive(true); 
            }
        }
    }

    public void MakeBigger(GameObject thisB)
    {
        // ������ �����
        ch = Instantiate(thisB, transform);

        Destroy(ch.transform.GetComponent<Button>());
        var textik = ch.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
        if (textik == "���� �� ����") { ind = 0; }
        if (textik == "���� �� ����") { ind = 1; }
        Destroy(ch.transform.GetChild(1).gameObject);

        // ���������� �������� ������� � ������ ������
        buttonPos = thisB.transform.position;
        var originalRt = thisB.GetComponent<RectTransform>();
        buttonSize = new Vector2(370, 370);

        // ������ �� ����� ������
        ch.transform.position = buttonPos;

        var rt = ch.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = buttonSize;   // �������� � ���������� �������

        // ���� ����
        startPos = buttonPos;
        targetPos = original.position;

        startSize = buttonSize;
        targetSize = new Vector2(970, 570);   // ������� ������

        butEx.SetActive(true);

        elapsed = 0f;
        isMoving = true;
    }

    public void clos()
    {
        if (ch == null) return;
        buts.gameObject.SetActive(false);
        // ���� ������� � ������
        startPos = ch.transform.position;      // ������ ������ (�������)
        targetPos = buttonPos;                 // ���� (�� ������)

        startSize = new Vector2(970, 570);
        targetSize = buttonSize;               // ������� � ��������� ������

        butEx.SetActive(false);

        elapsed = 0f;
        isMoving = true;
    }
}