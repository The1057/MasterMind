using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class clothItemList
{
    static float[] jeansSkinnyArgs = { 2300, 3450, 4600, 20, 60 };
    static float[] jeansClassicArgs = { 2000, 3000, 4000, 30, 80 };
    static float[] tshirtBaseArgs = { 600, 900, 1200, 50, 120 };
    static float[] tshirtOversizeArgs = { 1200, 1800, 2400, 15, 40 };
    static float[] sneakersSportArgs = { 3500, 5250, 7000, 25, 70 };
    static float[] sneakersCasualArgs = { 2800, 4200, 5600, 20, 55 };
    static float[] shirtOversizeArgs = { 1500, 2250, 3000, 20, 45 };
    static float[] shirtClassicArgs = { 1200, 1800, 2400, 20, 50 };
    static float[] formalDressArgs = { 4000, 6000, 8000, 10, 20 };
    static float[] casualDressArgs = { 2200, 3400, 4400, 20, 50 };

    public static List<Item> possibleItems = new()
    {
        new Item("Джинсы скинни",jeansSkinnyArgs),
        new Item("Джинсы классические",jeansClassicArgs),
        new Item("Футболка базовая",tshirtBaseArgs),
        new Item("Футболка оверсайз", tshirtOversizeArgs),
        new Item("Кроссовки спортивные",sneakersSportArgs),
        new Item("Кроссовки кэжул",sneakersCasualArgs),
        new Item("Рубашка оверсайз",shirtOversizeArgs),
        new Item("Рубашка классическая",shirtClassicArgs),
        new Item("Платье вечернее",formalDressArgs),
        new Item("Платье кэжуал",casualDressArgs),
    };

}
