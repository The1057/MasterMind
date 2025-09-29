using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using UnityEngine.Analytics;

public class PlayerGenderSelect : MonoBehaviour
{
    public playerDataClass playerData;

    public Button maleButton;
    public Button femaleButton;


    void Awake()
    {
    }

    void Start()
    {
    }
    public void selectMale()
    {
        playerData.data.player_gender = "M";
    }
    public void selectFemale()
    {
        playerData.data.player_gender = "F";
    }
}