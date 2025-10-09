using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class updateYearListScript : MonoBehaviour, ITickable
{
    public TMP_Dropdown yearList;
    public ClockScript clockScript;
    public void nextTurn(int month, int year)
    {
        //if(month == 1)
        //{
        //    yearList.options.Insert(0,new TMP_Dropdown.OptionData(year.ToString()));
        //}
    }
    public void updateYearList()
    {
        yearList.ClearOptions();
        yearList.value = -1;
        yearList.options.Add(new TMP_Dropdown.OptionData("Выберите год"));
        for (int i = clockScript.year-1; i > 0; i--)
        {
            yearList.AddOptions(new List<TMP_Dropdown.OptionData> {new TMP_Dropdown.OptionData(i.ToString()) });
        }
        yearList.RefreshShownValue();
    }
}
