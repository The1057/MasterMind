using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BusinessDropdown : MonoBehaviour
{
    [SerializeField] private GameObject dropdownPanel;
    [SerializeField] private float delayBetweenItems = 0.08f; // Задержка между строками
    [SerializeField] private float duration = 0.2f;           // Скорость анимации
    private bool isOpen = false;
    private List<CanvasGroup> items = new List<CanvasGroup>();

    private void Start()
    {
        // Сохраняем ссылки на все дочерние элементы и добавляем CanvasGroup, если их нет
        foreach (Transform child in dropdownPanel.transform)
        {
            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = child.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = 0;
            child.localScale = new Vector3(1, 0, 1);
            items.Add(cg);
        }

        dropdownPanel.SetActive(false);
    }

    public void ToggleDropdown()
    {
        StopAllCoroutines();
        isOpen = !isOpen;

        if (isOpen)
        {
            dropdownPanel.SetActive(true);
            StartCoroutine(AnimateOpen());
        }
        else
        {
            StartCoroutine(AnimateClose());
        }
    }

    private IEnumerator AnimateOpen()
    {
        for (int i = 0; i < items.Count; i++)
        {
            CanvasGroup cg = items[i];
            Transform tr = cg.transform;

            StartCoroutine(AnimateItem(cg, tr, true));
            yield return new WaitForSeconds(delayBetweenItems);
        }
    }

    private IEnumerator AnimateClose()
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            CanvasGroup cg = items[i];
            Transform tr = cg.transform;

            StartCoroutine(AnimateItem(cg, tr, false));
            yield return new WaitForSeconds(delayBetweenItems);
        }

        yield return new WaitForSeconds(duration + delayBetweenItems);
        dropdownPanel.SetActive(false);
    }

    private IEnumerator AnimateItem(CanvasGroup cg, Transform tr, bool opening)
    {
        float startAlpha = opening ? 0 : 1;
        float endAlpha = opening ? 1 : 0;
        Vector3 startScale = opening ? new Vector3(1, 0, 1) : Vector3.one;
        Vector3 endScale = opening ? Vector3.one : new Vector3(1, 0, 1);

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, normalized);
            tr.localScale = Vector3.Lerp(startScale, endScale, normalized);
            yield return null;
        }

        cg.alpha = endAlpha;
        tr.localScale = endScale;
    }
}
