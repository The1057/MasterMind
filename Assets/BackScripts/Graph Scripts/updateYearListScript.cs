using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class updateYearListScript : MonoBehaviour, ITickable
{
    public TMP_Dropdown yearList;
    int currentYear;
    public void nextTurn(int month, int year)
    {
        currentYear = year;
        //if(month == 1)
        //{
        //    yearList.options.Insert(0,new TMP_Dropdown.OptionData(year.ToString()));
        //}
    }
    public void updateYearList()
    {
        yearList.ClearOptions();
        for (int i = currentYear; i > 0; i++)
        {
            yearList.AddOptions(new List<TMP_Dropdown.OptionData> {new TMP_Dropdown.OptionData(i.ToString()) });
        }
    }
}
