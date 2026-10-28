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
    private Vector3 buttonPos;          //  запоминаем, откуда приехали
    private Vector2 startSize;
    private Vector2 targetSize;
    private Vector2 buttonSize;         //  исходный размер кнопки

    private void Update()
    {
        if (!isMoving || ch == null) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        // Позиция
        ch.transform.position = Vector3.Lerp(startPos, targetPos, t);

        // Размер (если хочешь анимировать и его)
        var rt = ch.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = Vector2.Lerp(startSize, targetSize, t);

        if (t >= 1f)
        {
            isMoving = false;
            ch.transform.position = targetPos;

            if (rt != null)
                rt.sizeDelta = targetSize;

            // Уничтожаем только когда вернулись обратно к кнопке
            if (Vector3.Distance(targetPos, buttonPos) < 0.1f)
            {
                Destroy(ch);
                ch = null;
            }
        }
    }

    public void MakeBigger(GameObject thisB)
    {
        // Создаём копию
        ch = Instantiate(thisB, transform);

        Destroy(ch.transform.GetComponent<Button>());
        Destroy(ch.transform.GetChild(1).gameObject);

        // Запоминаем исходную позицию и размер кнопки
        buttonPos = thisB.transform.position;
        var originalRt = thisB.GetComponent<RectTransform>();
        buttonSize = new Vector2(370, 370);

        // Ставим на место кнопки
        ch.transform.position = buttonPos;

        var rt = ch.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = buttonSize;   // начинаем с маленького размера

        // Куда едем
        startPos = buttonPos;
        targetPos = original.position;

        startSize = buttonSize;
        targetSize = new Vector2(970, 570);   // большой размер

        butEx.SetActive(true);

        elapsed = 0f;
        isMoving = true;
    }

    public void clos()
    {
        if (ch == null) return;

        // Едем обратно к кнопке
        startPos = ch.transform.position;      // откуда сейчас (большой)
        targetPos = buttonPos;                 // куда (на кнопку)

        startSize = new Vector2(970, 570);
        targetSize = buttonSize;               // обратно в маленький размер

        butEx.SetActive(false);

        elapsed = 0f;
        isMoving = true;
    }
}