using TMPro;
using UnityEngine;

public class MoneyGemUIUpdate : MonoBehaviour
{
    public GameObject moneyUI;
    public GameObject gemUI;

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
        moneyUI.GetComponent<TextMeshProUGUI>().text = saveData.MoneyData.player_money.ToString();
        gemUI.GetComponent<TextMeshProUGUI>().text = saveData.MoneyData.player_gems.ToString();
    }
}
