using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGemUIUpdate : MonoBehaviour
{
    public List<GameObject> moneyUI;
    public List<GameObject> gemUI;

    public saveLoadManager saveLoadManager;

    saveData saveData;

    public void Update()
    {
        updateUI();
    }

    [ContextMenu("Update Money and Gems")]
    public void updateUI()
    {
        saveData = saveLoadManager.loadData();
        foreach (var moneyUI in moneyUI)
        {
            moneyUI.GetComponent<TextMeshProUGUI>().text = saveData.MoneyData.player_money.ToString();
        }
        foreach (var gemUI in gemUI)
        {
            gemUI.GetComponent<TextMeshProUGUI>().text = saveData.MoneyData.player_gems.ToString();
        }
    }
}
