using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class bookItemList
{
    static float[] argsComic = { 200, 260, 320, 30, 80};
    static float[] argsEncic = { 600, 780, 960, 10, 30 };
    static float[] argsChildBook = { 250, 325, 400, 40, 120 };
    static float[] argsFictionBook = { 180, 234, 288, 30, 90 };
    static float[] argsStudyBook = { 400, 520, 640, 20, 50 };
    static float[] argsDiary = { 200, 260, 320, 30, 80 };
    static float[] argsNotepad = { 120, 156, 192, 50, 150 };
    static float[] argsBoardGame = { 700, 910, 1120, 15, 40 };
    static float[] argsPostcard = { 50, 65, 80, 60, 150 };

    public static List<Item> possibleItems = new() 
    { 
        new Item("Комикс", argsComic),
        new Item("Энциклопедия",argsEncic),
        new Item("Книга Детская",argsChildBook),
        new Item("Книга художественная",argsFictionBook),
        new Item("Учебник школьный",argsStudyBook),
        new Item("Ежедневник",argsDiary),
        new Item("Блокнот",argsNotepad),
        new Item("Настольная игра",argsBoardGame),
        new Item("Открытка",argsPostcard)
    };
}
