using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class interfaceUpdateScript : MonoBehaviour
{
    public int number;
    //public Text testUI;
    //public Text moneyDisplay;

    public ClockScript clock;
    public MoneyScript money;

    public GameObject debugButtons;
    void Start()
    {
        clock = GameObject.FindGameObjectWithTag("ClockTag").GetComponent<ClockScript>();
        money = GameObject.FindGameObjectWithTag("MoneyTag").GetComponent<MoneyScript>();
        addNumber();
    }
    void Update()
    {
        //testUI.text = clock.getYear().ToString() + " " + clock.getMonth().ToString();
        //moneyDisplay.text = money.getMoney().ToString();
    }


    [ContextMenu("Add to number")]
    public void addNumber()
    {
        number++;
        //testUI.text = number.ToString();
        //moneyDisplay.text = number.ToString();
    }

    [ContextMenu("Toggle Debug buttons")]
    public void toggleDebugButtons()
    {
        debugButtons.SetActive(!debugButtons.activeSelf);
    }
}
