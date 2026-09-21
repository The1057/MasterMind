using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class windowMarketing : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textAbout;
    [SerializeField] private TextMeshProUGUI features;
    [SerializeField] private GameObject windowsxd;
    [SerializeField] private TextMeshProUGUI textCost;

    [Header("Вид рекламы")]
    public int adTypeIndex;

    [Header("Названия")]
    public List<string> names;

    [Header("Описания")] 
    public List<string> textsAbout;

    [Header("Изменения")]
    public List<string> textsFeatures;

    [Header("Цены")]
    public List<string> costs;

    public AdvertisingManager advertisingManager;
    public MoneyScript moneyScript;

    public List<AdvertisingManager.UpgradeEffect> upgradeEffects = new List<AdvertisingManager.UpgradeEffect>();

    [System.Serializable] 
    public class spiskiObjects { public List<GameObject> gameObjects = new List<GameObject>(); }
    public List<spiskiObjects> outline = new List<spiskiObjects>();

    [System.Serializable]
    public class listnums { public List<int> gameObjects2 = new List<int>(); }
    public List<listnums> svyazi = new List<listnums>();

    public List<bool> activeN = new List<bool>();
    
    public float curCost;
    public int number;

    void Start()
    {
        LoadPurchasedNodes();
    }

    public void ClickBut(int num) {
        number = num - 1;
        textName.text = names[number];
        textAbout.text = textsAbout[number];
        features.text = textsFeatures[number];
        textCost.text = costs[number];
        curCost = int.Parse(costs[number]);
    }

    public void openWindow()
    {
        windowsxd.SetActive(true);
    }
    public void closeWindow()
    {
        windowsxd.SetActive(false);
    }
    public void buyAd() {
        if (!checkCan()) { Debug.Log("ты еще слаб"); }
        else if (moneyScript.getMoney() >= curCost)
        {
            moneyScript.addMoney(-curCost);
            activeN[number] = true;
            advertisingManager.SetPurchasedNodes(adTypeIndex, activeN);
            ApplyUpgrade(number);
            foreach (GameObject outl in outline[number].gameObjects)
            {
                var o = outl.GetComponent<Outline>();
                o.effectColor = new Color(o.effectColor.r, o.effectColor.g, o.effectColor.b, 1f);
            }
        }
        else {
            Debug.Log("ой, денюжек не хватает(");
        }
    }
    public bool checkCan()
    {
        foreach (int nomer in svyazi[number].gameObjects2)
        {
            if (!activeN[nomer])
            {
                return false;
            }
        }
        return true;
    }
    void ApplyUpgrade(int upgradeIndex)
    {
        var effect = upgradeEffects[upgradeIndex];
        advertisingManager.ApplyEffect(adTypeIndex, effect);
    }
    public void LoadPurchasedNodes()
    {
        var saved = advertisingManager.GetPurchasedNodes(adTypeIndex);

        // Подстраиваем размер списка, если нужно
        while (activeN.Count < saved.Count)
            activeN.Add(false);

        for (int i = 0; i < saved.Count && i < activeN.Count; i++)
        {
            activeN[i] = saved[i];

            // Восстанавливаем визуал (обводку)
            if (activeN[i] && i < outline.Count)
            {
                foreach (GameObject outl in outline[i].gameObjects)
                {
                    var o = outl.GetComponent<Outline>();
                    if (o != null)
                    {
                        o.effectColor = new Color(o.effectColor.r, o.effectColor.g, o.effectColor.b, 1f);
                    }
                }
            }
        }
    }
}
