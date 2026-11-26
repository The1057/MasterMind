using UnityEngine;
using UnityEngine.UI;

public class Bigger : MonoBehaviour
{
    [SerializeField] private Transform original;      // куда летит кнопка
    [SerializeField] private GameObject butEx;        // кнопка "закрыть"
    [SerializeField] private Button[] menuButtons;    // кнопки, по порядку
    [SerializeField] private GameObject[] submenus;   // подменю, в том же порядке

    private bool isMoving = false;
    private Vector3 startPos, targetPos;
    private Vector2 startSize, targetSize;
    private Vector3 buttonPos;
    private Vector2 buttonSize = new Vector2(370, 370);
    private Vector2 bigSize = new Vector2(970, 570);
    private float duration = 0.3f;
    private float elapsed = 0f;

    private GameObject ch;      // летающая копия кнопки
    private int currentIndex = -1;

    private void Start()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            int index = i; // важно: копия для замыкания
            menuButtons[i].onClick.AddListener(() => MakeBigger(index));
        }
    }

    private void Update()
    {
        if (!isMoving || ch == null) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        ch.transform.position = Vector3.Lerp(startPos, targetPos, t);
        var rt = ch.GetComponent<RectTransform>();
        if (rt != null) rt.sizeDelta = Vector2.Lerp(startSize, targetSize, t);

        if (t < 1f) return;

        isMoving = false;

        // Анимация закрытия закончилась — убираем копию
        if (Vector3.Distance(targetPos, buttonPos) < 0.1f)
        {
            Destroy(ch);
            ch = null;
            currentIndex = -1;
        }
        else
        {
            // Анимация открытия закончилась — показываем нужное подменю
            var sub = submenus[currentIndex];
            sub.transform.SetAsLastSibling();
            sub.SetActive(true);
        }
    }

    public void MakeBigger(int index)
    {
        if (ch != null) return; // уже что-то открыто/анимируется

        currentIndex = index;
        GameObject thisB = menuButtons[index].gameObject;

        ch = Instantiate(thisB, transform);
        Destroy(ch.GetComponent<Button>());
        if (ch.transform.childCount > 1)
            Destroy(ch.transform.GetChild(1).gameObject); // текст на копии не нужен

        buttonPos = thisB.transform.position;
        ch.transform.position = buttonPos;
        ch.GetComponent<RectTransform>().sizeDelta = buttonSize;

        startPos = buttonPos;
        targetPos = original.position;
        startSize = buttonSize;
        targetSize = bigSize;

        butEx.SetActive(true);
        elapsed = 0f;
        isMoving = true;
    }

    public void clos()
    {
        if (ch == null || currentIndex < 0) return;

        submenus[currentIndex].SetActive(false);

        startPos = ch.transform.position;
        targetPos = buttonPos;
        startSize = bigSize;
        targetSize = buttonSize;

        butEx.SetActive(false);
        elapsed = 0f;
        isMoving = true;
    }
}