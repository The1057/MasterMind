using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class on_click_script : MonoBehaviour
{
    public interfaceUpdateScript ui;
    void Start()
    {
        ui = GameObject.FindGameObjectWithTag("UIupdaterTag").GetComponent<interfaceUpdateScript>();
    }

    void OnMouseDown()
    {
        ui.addNumber();
        print("click!");
    }
}
