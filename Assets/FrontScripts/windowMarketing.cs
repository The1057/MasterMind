using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class windowMarketing : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textAbout;
    [SerializeField] private TextMeshProUGUI features;
    [SerializeField] private GameObject windowsxd;

    [Header("Названия")]
    public List<string> names;

    [Header("Описания")] 
    public List<string> textsAbout;

    [Header("Изменения")]
    public List<string> textsFeatures;

    public void ClickBut(int num) {
        textName.text = names[num-1];
        textAbout.text = textsAbout[num-1];
        features.text = textsFeatures[num-1];
    }

    public void openWindow()
    {
        windowsxd.SetActive(true);
    }
    public void closeWindow()
    {
        windowsxd.SetActive(false);
    }
}
