using TMPro;
using UnityEngine;

public class UltimateTextUpdateScript : MonoBehaviour
{
    [Header("Required objects")]
    public MoneyScript MoneyScript;
    public playerDataClass playerDataClass;

    [Header("Text")]
    public string nameText = "Èìÿ: ";

    [Header("Fields to fill")]
    public TextMeshProUGUI nameField;
    public TextMeshProUGUI moneyField;
    public TextMeshProUGUI gemField;
    void Start()
    {
    }

    void Update()
    {
        updateMoney();
        updateName();
    }
    void updateMoney()
    {
        moneyField.text = MoneyScript.player_money.ToString();
        gemField.text = MoneyScript.player_gems.ToString();
    }
    void updateName()
    {
        nameField.text = nameText + playerDataClass.data.player_name;
    }
}
