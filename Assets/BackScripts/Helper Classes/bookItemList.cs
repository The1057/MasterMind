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
        new Item(0,"Комикс", argsComic),
        new Item(1,"Энциклопедия",argsEncic),
        new Item(2,"Книга Детская",argsChildBook),
        new Item(3,"Книга художественная",argsFictionBook),
        new Item(4,"Учебник школьный",argsStudyBook),
        new Item(5,"Ежедневник",argsDiary),
        new Item(6,"Блокнот",argsNotepad),
        new Item(7,"Настольная игра",argsBoardGame),
        new Item(8,"Открытка",argsPostcard)
    };
}
