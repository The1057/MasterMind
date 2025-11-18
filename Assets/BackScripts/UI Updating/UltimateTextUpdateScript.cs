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

    float money;
    float gems;
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
        money = MoneyScript.player_money;
        gems = MoneyScript.player_gems;
        if(money > 1000000)
        {
            money /= 100000;
            moneyField.text = (Mathf.RoundToInt(money) / 10f).ToString() + " Ì";
        }
        else if (money > 1000)
        {
            money /= 100;
            moneyField.text = (Mathf.RoundToInt(money)/10f).ToString() + " ê";
        }
        else
        {
            moneyField.text = money.ToString();
        }

        if (gems > 1000000)
        {
            gems /= 100000;
            gemField.text = (Mathf.RoundToInt(gems) / 10).ToString() + " Ì";
        }
        else if (gems > 1000)
        {
            gems /= 100;
            gemField.text = (Mathf.RoundToInt(gems)/10).ToString() + " ê";
        }
        else
        {
            gemField.text = gems.ToString();
        }        
    }
    void updateName()
    {
        nameField.text = nameText + playerDataClass.data.player_name;
    }
}
