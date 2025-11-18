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
        new Item(0, "Багет", baguetteArgs),
        new Item(1,"Хлеб цельнозерновой",breadWheatArgs),
        new Item(2,"Хлеб ржаной",breadRyeArgs),
        new Item(3,"Круассан классический",croissantArgs),
        new Item(4,"Круассан с начинкой",croissantFilledArgs),
        new Item(5,"Торт шоколадный, кг",cakeChocoArgs),
        new Item(6,"Торт ягодный, кг",cakeBerryArgs),
        new Item(7,"Пирожное эклер",eclairArgs),
        new Item(8,"Пирожное медовик",cakeHoneyArgs),
        new Item(9,"Печенье (ассорти)",cookieArgs)
    };

}
