using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class breadItemList
{
    static float[] baguetteArgs = { 40, 80, 120, 200, 500};
    static float[] breadWheatArgs = { 50, 100, 150, 150, 400 };
    static float[] breadRyeArgs = { 45, 90, 135, 100, 300 };
    static float[] croissantArgs = { 45, 90, 135, 100, 300 };
    static float[] croissantFilledArgs = { 60, 180, 200, 80, 250 };
    static float[] cakeChocoArgs = { 350, 700, 1050, 10, 50 };
    static float[] cakeBerryArgs = { 370, 740, 1110, 10, 40 };
    static float[] eclairArgs = { 35, 70, 105, 150, 400 };
    static float[] cakeHoneyArgs = { 40, 80, 120, 120, 300 };
    static float[] cookieArgs = { 200, 400, 600, 50, 150 };

    public static List<Item> possibleItems = new()
    {
        new Item("Багет", baguetteArgs),
        new Item("Хлеб цельнозерновой",breadWheatArgs),
        new Item("Хлеб ржаной",breadRyeArgs),
        new Item("Круассан классический",croissantArgs),
        new Item("Круассан с начинкой",croissantFilledArgs),
        new Item("Торт шоколадный, кг",cakeChocoArgs),
        new Item("Торт ягодный, кг",cakeBerryArgs),
        new Item("Пирожное эклер",eclairArgs),
        new Item("Пирожное медовик",cakeHoneyArgs),
        new Item("Печенье (ассорти)",cookieArgs)
    };

}
