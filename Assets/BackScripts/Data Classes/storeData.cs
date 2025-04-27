using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class storeData
{
    public string name = "";

    public List<Item> items = new List<Item>();
    public float adModifier = 1.1f;

    public List<Vector2> points4Graph = new List<Vector2>();
    public List<Vector2> points4GraphFinal = new List<Vector2>();
    public int storeID = 0;
}
