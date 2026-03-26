using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class mistakeFiller
{
    public Sprite correctWord;
    public List<Sprite> incorrectWords;
}
public class DocMistakeMinigame : MonoBehaviour
{
    public Sprite outline;
    public GameObject correctBlocker;
    public GameObject incorrectBlocker;
    public GameObject rt;
    List<Image> outlines = new();
    public List<RectTransform> slots;
    public List<mistakeFiller> mistakeFillers;
    public bool[] correctIncorrect;
    public bool[] clicked;

    private void Start()
    {          
        clicked = new bool[slots.Count];
        correctIncorrect = new bool[slots.Count];
        for(int i =0;i<slots.Count;i++)
        {
            float r = UnityEngine.Random.value;
            if (r > 0.5f)
            {
                putImageInSlot(slots[i], mistakeFillers[i].correctWord);
                correctIncorrect[i] = true;
            }
            else
            {                
                var rnd = UnityEngine.Random.Range(0, mistakeFillers[i].incorrectWords.Count);
                putImageInSlot(slots[i], mistakeFillers[i].incorrectWords[rnd]);
                correctIncorrect[i] = false;
            }
            var go = Instantiate(rt, slots[i]);
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<Image>().sprite = outline;
            go.GetComponent<Image>().raycastTarget = false;
            go.GetComponent<RectTransform>().sizeDelta *= 1.1f;
            go.gameObject.SetActive(false);
            outlines.Add(go.GetComponent<Image>());
        }
    }
    void putImageInSlot(RectTransform slot, Sprite image)
    {
        var go = Instantiate(rt,slot);
        go.AddComponent<Image>().sprite = image;
        go.transform.localPosition = Vector3.zero;
        go.AddComponent<Button>().onClick.AddListener(() => onClick(slot));
    }

    void onClick(RectTransform slot)
    {
        int index = slots.FindIndex(a => a == slot);
        clicked[index] = !clicked[index];
        outlines[index].gameObject.SetActive(clicked[index]);
    }

    public void submit() 
    {
        int correctAnswers = 0;
        int incorrectAnswers = 0;
        for (int i = 0; i < slots.Count; i++)
        {
            if (clicked[i] ^ correctIncorrect[i])
            {
                Instantiate(correctBlocker, slots[i]).transform.localPosition = Vector3.zero;
                correctAnswers++;
            }
            else
            {
                Instantiate(incorrectBlocker, slots[i]).transform.localPosition = Vector3.zero;
                incorrectAnswers++;
            }
        }
        print($"Correct answers: {correctAnswers}\nIncorrect answers: {incorrectAnswers}");
    }
}
