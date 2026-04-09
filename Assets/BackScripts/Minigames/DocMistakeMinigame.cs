using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class mistakeFiller
{
    public RectTransform slot;
    public Sprite correctWord;
    public List<Sprite> incorrectWords;
}
public class DocMistakeMinigame : MonoBehaviour
{
    public Sprite outline;
    public GameObject correctBlocker;
    public GameObject incorrectBlocker;
    public List<GameObject> blockers;

    public GameObject rt;
    List<Image> outlines = new();
    //public List<RectTransform> slots;
    public List<mistakeFiller> mistakeFillers;



    public bool[] correctIncorrect;
    public bool[] clicked;

    public bool firstClick = true;

    private void Start()
    {
        clicked = new bool[mistakeFillers.Count];
        correctIncorrect = new bool[mistakeFillers.Count];
        for (int i = 0; i < mistakeFillers.Count; i++)
        {
            float r = UnityEngine.Random.value;
            if (r > 0.5f)
            {
                putImageInSlot(mistakeFillers[i].slot, mistakeFillers[i].correctWord);
                correctIncorrect[i] = true;
            }
            else
            {
                var rnd = UnityEngine.Random.Range(0, mistakeFillers[i].incorrectWords.Count);
                putImageInSlot(mistakeFillers[i].slot, mistakeFillers[i].incorrectWords[rnd]);
                correctIncorrect[i] = false;
            }
            var go = Instantiate(rt, mistakeFillers[i].slot);
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<Image>().sprite = outline;
            go.GetComponent<Image>().raycastTarget = false;
            go.GetComponent<RectTransform>().sizeDelta = mistakeFillers[i].slot.sizeDelta * 1.3f;
            go.gameObject.SetActive(false);
            outlines.Add(go.GetComponent<Image>());
        }
    }
    void putImageInSlot(RectTransform slot, Sprite image)
    {
        var go = Instantiate(rt, slot);
        go.AddComponent<Image>().sprite = image;
        go.transform.localPosition = Vector3.zero;
        go.GetComponent<RectTransform>().sizeDelta = slot.sizeDelta;
        go.AddComponent<Button>().onClick.AddListener(() => onClick(slot));
    }

    void onClick(RectTransform slot)
    {
        if (firstClick)
        {
            int index = FindSlot(slot);
            clicked[index] = !clicked[index];
            outlines[index].gameObject.SetActive(clicked[index]);
        }
    }

    int FindSlot(RectTransform slot)
    {
        for (int i = 0; i < mistakeFillers.Count; i++)
        {
            if (mistakeFillers[i].slot == slot) return i;
        }
        return 0;
    }

    public void submit()
    {
        if (firstClick)
        {
            int correctAnswers = 0;
            int incorrectAnswers = 0;
            for (int i = 0; i < mistakeFillers.Count; i++)
            {
                if (clicked[i] ^ correctIncorrect[i])
                {
                    var a = Instantiate(correctBlocker, mistakeFillers[i].slot);
                    a.transform.localPosition = Vector3.zero;
                    a.GetComponent<RectTransform>().sizeDelta = mistakeFillers[i].slot.sizeDelta;
                    blockers.Add(a);
                    correctAnswers++;
                }
                else
                {
                    var a = Instantiate(incorrectBlocker, mistakeFillers[i].slot);
                    a.transform.localPosition = Vector3.zero;
                    a.GetComponent<RectTransform>().sizeDelta = mistakeFillers[i].slot.sizeDelta;
                    blockers.Add(a);
                    incorrectAnswers++;
                }
            }
            print($"Correct answers: {correctAnswers}\nIncorrect answers: {incorrectAnswers}");
            firstClick = false;
        }
    }

    [ContextMenu("Reset")]
    public void Reset()
    {
        firstClick = true;
        foreach (var blocker in blockers)
        {
            Destroy(blocker);
        }
        for (int i = 0; i < mistakeFillers.Count; ++i)
        {
            outlines[i].gameObject.SetActive(false);
            clicked[i] = false;
        }
    }
}
